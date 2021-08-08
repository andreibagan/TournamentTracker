﻿using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TournamentTracker.DataAccess;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public class SqlConnector : IDataConnection
    {
        private readonly ISqlDataAccess _db;
        private const string connectionStringName = "Tournaments";

        public SqlConnector(ISqlDataAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Saves a new prize to the database
        /// </summary>
        /// <param name="model">The prize information.</param>
        /// <returns>The prize information, including the unique identifier.</returns>
        public void CreatePrize(PrizeModel model)
        {
            _db.SaveData("dbo.spPrizes_Insert", model, connectionStringName, true);
        }

        public PrizeModel GetPrizeById(int PrizeId)
        {
            return _db.LoadDate<PrizeModel, dynamic>("dbo.spPrizes_GetById", new { PrizeId }, connectionStringName, true).First();
        }

        public void CreatePerson(PersonModel model)
        {
            _db.SaveData("dbo.spPeople_Insert", new { FirstName = model.FirstName, LastName = model.LastName, EmailAddress = model.EmailAddress, CellphoneNumber = model.CellphoneNumber }, connectionStringName, true);
        }

        public PersonModel GetPersonById(int PersonId)
        {
            return _db.LoadDate<PersonModel, dynamic>("dbo.spPeople_GetById", new { PersonId }, connectionStringName, true).First();
        }
    }
}
