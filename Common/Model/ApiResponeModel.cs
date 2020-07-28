namespace Common.Model
{
    public class ApiResponeModel
    {
        public bool IsValid { get; set; }
        public string ErrorMsg { get; set; }
        public dynamic Data { get; set; }
    }
}
