using BankAccountServices.Entities;
using BankAccountServices.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BankAccountServices.DTOs.BankAccount.SavingBankAccount
{
    public class SavingBankAccountCreateDTO:BankAccountCreateDTO
    {

        public double InterestRate { get; set; }


    }
}
