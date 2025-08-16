namespace BankAccountServices.DTOs
{
	public class Retour
	{
		public Retour()
		{
			Code = CodeRetour.Ko;
			Message = "Ko";
		}
		public long ID { get; set; }
		public string Message {  get; set; }
		public CodeRetour Code { get; set; }
	}
	public enum CodeRetour
	{
		Ok = 1,
		Ko = -1
	}
}
