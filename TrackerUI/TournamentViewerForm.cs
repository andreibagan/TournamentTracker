using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using TournamentTracker;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class TournamentViewerForm : Form
    {
        private readonly TournamentModel _tournament;
        private readonly BindingList<int> _rounds = new BindingList<int>();
        private BindingList<MatchupModel> _matchups = new BindingList<MatchupModel>();

        public TournamentViewerForm(TournamentModel tournament)
        {
            InitializeComponent();

            _tournament = tournament;

            _tournament.OntournamentComplete += _tournament_OntournamentComplete;

            WireUpLists();

            LoadFormData();
            LoadRounds();
        }

        private void _tournament_OntournamentComplete(object sender, DateTime e)
        {
            this.Close();
        }

        private void WireUpLists()
        {
            roundDropDown.DataSource = _rounds;

            matchupListBox.DataSource = _matchups;
            matchupListBox.DisplayMember = "DisplayName";
        }

        private void LoadFormData()
        {
            tournamentName.Text = _tournament.TournamentName;
        }

        private void LoadRounds()
        {
            _rounds.Clear();

            _rounds.Add(1);
            int currRound = 1;

            foreach (List<MatchupModel> matchups in _tournament.Rounds)
            {
                if (matchups.First().MatchupRound > currRound)
                {
                    currRound = matchups.First().MatchupRound;
                    _rounds.Add(currRound);
                }
            }

            LoadMatchups(1);
        }

        private void roundDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMatchups((int)roundDropDown.SelectedItem);
        }

        private void LoadMatchups(int round)
        {
            foreach (List<MatchupModel> matchups in _tournament.Rounds)
            {
                if (matchups.First().MatchupRound == round)
                {
                    _matchups.Clear();
                    foreach (MatchupModel matchup in matchups)
                    {
                        if (matchup.Winner == null || !unplayedonlyCheckbox.Checked)
                        {
                            _matchups.Add(matchup); 
                        }
                    }
                }
            }

            if (_matchups.Count > 0)
            {
                LoadMatchup(_matchups.First()); 
            }

            DisplayMatchupInfo();
        }

        private void DisplayMatchupInfo()
        {
            bool isVisible = _matchups.Count > 0;

            teamOneNameLabel.Visible = isVisible;
            teamOneScoreLabel.Visible = isVisible;
            teamOneScoreValue.Visible = isVisible;
            teamTwoNameLabel.Visible = isVisible;
            teamTwoScoreLabel.Visible = isVisible;
            teamTwoScoreValue.Visible = isVisible;
            versusLabel.Visible = isVisible;
            scoreButton.Visible = isVisible; 
        }

        private void LoadMatchup(MatchupModel matchup)
        {
            if (matchup == null)
            {
                return;
            }

            for (int i = 0; i < matchup.Entries.Count; i++)
            {
                if (i == 0)
                {
                    if (matchup.Entries[i].TeamCompeting != null)
                    {
                        teamOneNameLabel.Text = matchup.Entries[i].TeamCompeting.TeamName;
                        teamOneScoreValue.Text = matchup.Entries[i].Score.ToString();

                        teamTwoNameLabel.Text = "<bye>";
                        teamTwoScoreValue.Text = "0";
                    }
                    else
                    {
                        teamOneNameLabel.Text = "Not Yet Set";
                        teamOneScoreValue.Text = "";
                    }
                }

                if (i == 1)
                {
                    if (matchup.Entries[i].TeamCompeting != null)
                    {
                        teamTwoNameLabel.Text = matchup.Entries[i].TeamCompeting.TeamName;
                        teamTwoScoreValue.Text = matchup.Entries[i].Score.ToString();
                    }
                    else
                    {
                        teamTwoNameLabel.Text = "Not Yet Set";
                        teamTwoScoreValue.Text = "";
                    }
                }
            }
        }

        private void matchupListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMatchup((MatchupModel)matchupListBox.SelectedItem);
        }

        private void unplayedonlyCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            LoadMatchups((int)roundDropDown.SelectedItem);
        }

        private void scoreButton_Click(object sender, EventArgs e)
        {
            string errorMessage = ValidateData();
            if (errorMessage.Length > 0)
            {
                MessageBox.Show($"Input Error: {errorMessage}");
                return;
            }

            MatchupModel matchup = (MatchupModel)matchupListBox.SelectedItem;
            double teamOneScore = 0;
            double teamTwoScore = 0;

            for (int i = 0; i < matchup.Entries.Count; i++)
            {
                if (i == 0)
                {
                    if (matchup.Entries[i].TeamCompeting != null)
                    {
                        bool scoreValid = double.TryParse(teamOneScoreValue.Text, out teamOneScore);

                        if (scoreValid)
                        {
                            matchup.Entries[i].Score = teamOneScore; 
                        }
                        else
                        {
                            MessageBox.Show("Please enter a valid score for team 1.", "Invalid value", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }

                if (i == 1)
                {
                    if (matchup.Entries[i].TeamCompeting != null)
                    {
                        bool scoreValid = double.TryParse(teamTwoScoreValue.Text, out teamTwoScore);

                        if (scoreValid)
                        {
                            matchup.Entries[i].Score = teamTwoScore;
                        }
                        else
                        {
                            MessageBox.Show("Please enter a valid score for team 2.", "Invalid value", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
            }

            try
            {
                TournamentLogic.UpdateTournamentResults(_tournament);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"The application had the following error: {ex.Message}");
                return;
            }

            LoadMatchups((int)roundDropDown.SelectedItem);
        }

        private string ValidateData()
        {
            string output = "";

            double teamOneScore = 0;
            double teamTwoScore = 0;

            bool scoreOneValid = double.TryParse(teamOneScoreValue.Text, out teamOneScore);
            bool scoreTwoValid = double.TryParse(teamTwoScoreValue.Text, out teamTwoScore);

            if (!scoreOneValid)
            {
                output = "The Score One value is not a valid number.";
            }
            else if (!scoreTwoValid)
            {
                output = "The Score Two value is not a valid number.";
            }
            else if (teamOneScore == 0 && teamTwoScore == 0)
            {
                output = "You did not enter a score for either team.";
            }
            else if (teamOneScore == teamTwoScore)
            {
                output = "We do not allow ties in this application.";
            }

            return output;
        }
    }
}
