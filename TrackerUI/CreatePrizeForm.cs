using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class CreatePrizeForm : Form
    {
        private readonly IPrizeRequester _callingForm;

        public CreatePrizeForm(IPrizeRequester callingForm)
        {
            InitializeComponent();

            _callingForm = callingForm;
        }

        private void createPrizeButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                PrizeModel model = new PrizeModel(placeNumberValue.Text, placeNameValue.Text, prizeAmountValue.Text, prizePercentageValue.Text);

                GlobalConfig.Connection.CreatePrize(model);

                _callingForm.PrizeComplete(model);

                this.Close();
            }
            else
            {
                MessageBox.Show("This form has invalid information. Please check it and try again.");
            }
        }

        private bool ValidateForm()
        {
            bool output = true;
            int placeNumber = 0;
            bool placeNumberValid= int.TryParse(placeNumberValue.Text, out placeNumber);

            if (!placeNumberValid)
            {
                output = false;
            }

            if (placeNumber < 1)
            {
                output = false;
            }

            if (String.IsNullOrWhiteSpace(placeNameValue.Text))
            {
                output = false;
            }

            decimal prizeAmount = 0;
            double prizePercentage = 0;

            bool prizeAmountValid = decimal.TryParse(prizeAmountValue.Text, out prizeAmount);
            bool prizePercentageValid = double.TryParse(prizePercentageValue.Text, out prizePercentage);

            if (!prizeAmountValid || !prizePercentageValid)
            {
                output = false;
            }

            if (prizeAmount <= 0 && prizePercentage <= 0)
            {
                output = false;
            }

            if (prizePercentage < 0 || prizePercentage > 100)
            {
                output = false;
            }

            return output;
        }
    }
}
