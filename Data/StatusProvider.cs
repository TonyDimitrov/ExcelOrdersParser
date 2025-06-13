namespace Werehouse.Data
{
    public class StatusProvider
    {
        public StatusProvider()
        {
            SetValues();
        }
        public HashSet<string> Completed { get; set; } = new(); 
        public HashSet<string> Delay { get; set; } = new();
        public HashSet<string> Partially { get; set; } = new();

        private void SetValues()
        {
            Completed.Add("изпълнена");
            Completed.Add("изпълнено");

            Delay.Add("отложена");
            Delay.Add("отложено");

            Partially.Add("частично");
            Partially.Add("частичнa");
        }
    }
}
