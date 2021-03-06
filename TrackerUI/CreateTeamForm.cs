using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.DataAccess;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class CreateTeamForm : Form
    {
        private readonly List<PersonModel> availableTeamMembers = GlobalConfig.Connection.GetAllPeople();
        private readonly List<PersonModel> selectedTeamMembers = new List<PersonModel>();
        private readonly ITeamRequester _teamRequester;

        public CreateTeamForm(ITeamRequester teamRequester)
        {
            InitializeComponent();

            WireUpLists();
            _teamRequester = teamRequester;
        }

        private void WireUpLists()
        {
            selectTeamMemberDropDown.DataSource = null;
            teamMembersListBox.DataSource = null;

            selectTeamMemberDropDown.DataSource = availableTeamMembers;
            selectTeamMemberDropDown.DisplayMember = "FullName";

            teamMembersListBox.DataSource = selectedTeamMembers;
            teamMembersListBox.DisplayMember = "FullName";
        }

        private void createMemberButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                PersonModel personModel = new PersonModel
                {
                    FirstName = firstNameValue.Text,
                    LastName = lastNameValue.Text,
                    EmailAddress = emailValue.Text,
                    CellphoneNumber = cellPhoneValue.Text
                };

                GlobalConfig.Connection.CreatePerson(personModel);

                selectedTeamMembers.Add(personModel); 

                WireUpLists();
                ResetForm();
            }
            else
            {
                MessageBox.Show("You need to fill in all of the fields.");
            }
        }

        private void ResetForm()
        {
            firstNameValue.Text = String.Empty;
            lastNameValue.Text = String.Empty;
            emailValue.Text = String.Empty;
            cellPhoneValue.Text = String.Empty;
        }

        private bool ValidateForm()
        {
            bool output = true;

            if (String.IsNullOrWhiteSpace(firstNameValue.Text))
            {
                output = false;
            }

            if (String.IsNullOrWhiteSpace(lastNameValue.Text))
            {
                output = false;
            }

            if (String.IsNullOrWhiteSpace(emailValue.Text))
            {
                output = false;
            }

            if (String.IsNullOrWhiteSpace(cellPhoneValue.Text))
            {
                output = false;
            }

            return output;
        }

        private bool ValidateTeam()
        {
            bool output = true;

            if (String.IsNullOrWhiteSpace(teamNameValue.Text))
            {
                output = false;
            }

            return output;
        }

        private void addMemberButton_Click(object sender, EventArgs e)
        {
            PersonModel person = (PersonModel)selectTeamMemberDropDown.SelectedItem;

            if (person != null)
            {
                availableTeamMembers.Remove(person);
                selectedTeamMembers.Add(person);

                WireUpLists();
            }
        }

        private void removeSelectedMemberButton_Click(object sender, EventArgs e)
        {
            PersonModel person = (PersonModel)teamMembersListBox.SelectedItem;

            if (person != null)
            {
                selectedTeamMembers.Remove(person);
                availableTeamMembers.Add(person);

                WireUpLists();
            }
        }

        private void createTeamButton_Click(object sender, EventArgs e)
        {
            if (ValidateTeam())
            {
                TeamModel team = new TeamModel();

                team.TeamName = teamNameValue.Text;
                team.TeamMembers = selectedTeamMembers;

                GlobalConfig.Connection.CreateTeam(team);

                _teamRequester.TeamComplete(team);

                Close();
            }
            else
            {
                MessageBox.Show("You need to fill in all of the fields.");
            }
        }
    }
}
