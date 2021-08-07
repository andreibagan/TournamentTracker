namespace TrackerLibrary
{
    public class TextConnection : IDataConnection
    {
        // TODO - Make the CreatePrize method actually save to the text file.
        public PrizeModel CreatePrize(PrizeModel model)
        {
            model.Id = 1;

            return model;
        }
    }
}
