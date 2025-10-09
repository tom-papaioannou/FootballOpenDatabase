namespace FootballOpenDatabase.Models.People
{
    public class Person
    {
        public Guid PersonID { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PlaceOfBirth { get; set; }
        public Guid? ContractID { get; set; }
    }
}
