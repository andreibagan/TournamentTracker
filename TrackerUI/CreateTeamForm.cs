using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.DataAccess;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class CreateTeamForm : Form
    {
        public CreateTeamForm()
        {
            InitializeComponent();
        }

        private void createMemberButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                PersonModel person = new PersonModel
                {
                    FirstName = firstNameValue.Text,
                    LastName = lastNameValue.Text,
                    EmailAddress = emailValue.Text,
                    CellphoneNumber = cellPhoneValue.Text
                };

                foreach (IDataConnection db in GlobalConfig.Connections)
                {
                    db.CreatePerson(person);
                }

                RefreshForm();
            }
            else
            {
                MessageBox.Show("You need to fill in all of the fields.");
            }
        }

        private void RefreshForm()
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
    }
}
