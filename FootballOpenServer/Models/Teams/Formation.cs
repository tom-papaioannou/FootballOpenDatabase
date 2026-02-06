namespace FootballOpenServer.Models.Teams
{
    public enum Formation
    {
        // Classic
        Four_Four_Two,
        Four_Three_Three,
        Three_Five_Two,
        Five_Three_Two,
        Four_Five_One,

        // 4 at the back variations
        Four_Two_Three_One,
        Four_Three_Two_One,
        Four_One_Four_One,
        Four_Four_One_One,

        // 3 at the back
        Three_Four_Three,
        Three_Four_Two_One,
        Three_Four_One_Two,

        // 5 at the back / wingbacks
        Five_Four_One,
        Five_Two_Three,
        Five_Three_One_One,

        // Asymmetric / uncommon / historical
        Four_Two_Two_Two,
        Four_Six_Zero,
        Three_Three_Four,
        Two_Three_Five
    }
}
