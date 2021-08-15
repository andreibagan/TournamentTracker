using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TournamentTracker;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class CreateTournamentForm : Form, IPrizeRequester, ITeamRequester
    {
        private readonly List<TeamModel> availableTeams = GlobalConfig.Connection.GetAllTeams();
        private readonly List<TeamModel> selectedTeams = new List<TeamModel>();
        private readonly List<PrizeModel> selectedPrizes = new List<PrizeModel>();

        public CreateTournamentForm()
        {
            InitializeComponent();

            WireUpLists();
        }

        private void WireUpLists()
        {
            selectTeamDropDown.DataSource = null;
            tournamentTeamsListBox.DataSource = null;
            prizesListBox.DataSource = null;

            selectTeamDropDown.DataSource = availableTeams;
            selectTeamDropDown.DisplayMember = "TeamName";

            tournamentTeamsListBox.DataSource = selectedTeams;
            tournamentTeamsListBox.DisplayMember = "TeamName";

            prizesListBox.DataSource = selectedPrizes;
            prizesListBox.DisplayMember = "PlaceName";
        }

        private bool ValidateForm()
        {
            if (String.IsNullOrWhiteSpace(tournamentNameValue.Text))
            {
                MessageBox.Show("You need to enter a valid Tournament Name.", "Invalid Tournament Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            decimal fee = 0;
            bool feeAcceptable = decimal.TryParse(EntryFeeValue.Text, out fee);

            if (!feeAcceptable)
            {
                MessageBox.Show("You need to enter a valid Entry Fee.", "Invalid Fee", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void addTeamButton_Click(object sender, EventArgs e)
        {
            TeamModel team = (TeamModel)selectTeamDropDown.SelectedItem;

            if (team != null)
            {
                availableTeams.Remove(team);
                selectedTeams.Add(team);

                WireUpLists();
            }
        }

        private void removeSelectedPlayerButton_Click(object sender, EventArgs e)
        {
            TeamModel team = (TeamModel)tournamentTeamsListBox.SelectedItem;

            if (team != null)
            {
                selectedTeams.Remove(team);
                availableTeams.Add(team);

                WireUpLists();
            }
        }

        private void removeSelectedPrizeButton_Click(object sender, EventArgs e)
        {
            PrizeModel prize = (PrizeModel)prizesListBox.SelectedItem;

            if (prize != null)
            {
                selectedPrizes.Remove(prize);
                WireUpLists();
            }
        }

        private void createPrizeButton_Click(object sender, EventArgs e)
        {
            CreatePrizeForm prizeForm = new CreatePrizeForm(this);
            prizeForm.Show();
        }

        public void PrizeComplete(PrizeModel model)
        {
            selectedPrizes.Add(model);
            WireUpLists();
        }

        private void createNewTeamLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CreateTeamForm teamForm = new CreateTeamForm(this);
            teamForm.Show();
        }

        public void TeamComplete(TeamModel model)
        {
            selectedTeams.Add(model);
            WireUpLists();
        }

        private void createTournamentButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                TournamentModel tournament = new TournamentModel();

                tournament.TournamentName = tournamentNameValue.Text;
                tournament.EntryFee = decimal.Parse(EntryFeeValue.Text);

                tournament.Prizes = selectedPrizes;
                tournament.EnteredTeams = selectedTeams;

                TournamentLogic.CreateRounds(tournament);

                GlobalConfig.Connection.CreateTournament(tournament);
            }
        }
    }
}
