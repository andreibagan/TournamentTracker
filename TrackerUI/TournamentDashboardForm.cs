using System.Collections.Generic;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class TournamentDashboardForm : Form
    {
        private readonly List<TournamentModel> tournaments = GlobalConfig.Connection.GetAllTournaments();

        public TournamentDashboardForm()
        {
            InitializeComponent();

            WireUpLists();
        }

        private void WireUpLists()
        {
            loadExistingTournamentDropDown.DataSource = null;
            loadExistingTournamentDropDown.DataSource = tournaments;
            loadExistingTournamentDropDown.DisplayMember = "TournamentName";
        }

        private void createTournamentButton_Click(object sender, System.EventArgs e)
        {
            CreateTournamentForm tournamentForm = new CreateTournamentForm();
            tournamentForm.Show();
        }

        private void loadTournamentButton_Click(object sender, System.EventArgs e)
        {
            TournamentModel selectedTournament = (TournamentModel)loadExistingTournamentDropDown.SelectedItem;
            TournamentViewerForm tournamentViewer = new TournamentViewerForm(selectedTournament);
            tournamentViewer.Show();
        }
    }
}
