using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Repository.Interface
{
	public interface IGenericRepository<T>
	{
		Task<T> Create(T entity);
		T Update(T entity);
		T Remove(int id);
		Task<T> Get(int id,params string[] includes);
		Task<IQueryable<T>> GetAll();
		Task SaveAsync();
		void Save();

	}
}
