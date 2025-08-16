namespace BankAccountServices.DTOs.BankAccount.CurrentBankAccount
{
    public class CurrentBankAccountCreateDTO:BankAccountCreateDTO
    {

        public double OverDraft { get; set; }

    }
}
