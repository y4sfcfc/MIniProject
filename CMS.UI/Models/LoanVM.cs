using CMS.DAL.Entities;
using CMS.DAL.Enums;

namespace CMS.UI.Models
{
    public class LoanVM:BaseEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal MonthlyPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public int Terms { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public Status Status { get; set; }
		public List<string> Statuses { get; set; }
		public List<int> TermList { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public List<Customer> Customers { get; set; }
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public List<Employee> Employees { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public List<Product> Products { get; set; }
        public LoanDetailVM LoanDetail { get; set; }
    }
}
