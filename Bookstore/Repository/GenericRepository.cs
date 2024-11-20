using Bookstore.Contex;

namespace Bookstore.Repository
{
    public class GenericRepository<T> where T : class 
    {
        protected BookstoreContext db;
        public GenericRepository(BookstoreContext db)
        {
            this.db = db;
        }
        public List<T> Selectall()
        {
            return db.Set<T>().ToList();
        }
        public T GetById(int id)
        {
            var x = db.Set<T>().Find(id);
            if (x == null)
            {
                return null;
            }
            return x;
        }
        public void Add(T Entity)
        {
            db.Set<T>().Add(Entity);
        }
        public void Delete(T Entity)
        {
            db.Set<T>().Remove(Entity);
        }
        public void Edit(T Entity)
        {
            db.Update(Entity);
        }
    }
}
