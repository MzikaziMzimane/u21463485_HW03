using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using u21463485_HW03.Models;

namespace u21463485_HW03.Controllers
{
    public class HomeController : Controller
    {
       

        private LibraryEntities db = new LibraryEntities();
        private int size = 20;

        public async Task<ActionResult> Index(int skipPage = 1)
        {
            var viewModel = new CombinedViewModel();
            viewModel.Authors = await db.authors.Include(b => b.books).ToListAsync();
            var myBooks = db.books.Include(a => a.author).Include(bo => bo.borrows).Include(ty => ty.type).OrderBy(b => b.bookId);
            viewModel.Books = await myBooks.Skip((skipPage - 1) * size).Take(size).ToListAsync();
            var myStudents = db.students.Include(bo => bo.borrows).OrderBy(s => s.studentId);
            viewModel.Students = await myStudents.Skip((skipPage - 1) * size).Take(size).ToListAsync();
            viewModel.Types = await db.types.Include(b => b.books).ToListAsync();
            viewModel.Borrows = await db.borrows.Include(b => b.book).Include(s => s.student).ToListAsync();

            ViewBag.Page = skipPage;
            ViewBag.skipStudent = (int)Math.Ceiling((double)myStudents.Count() / size);
            ViewBag.skipBook = (int)Math.Ceiling((double)myBooks.Count() / size);

            return View(viewModel);

        }


        public async Task<ActionResult> AuthorIndex()
        {
            return View(await db.authors.ToListAsync());
        }

        // GET: authors/Details/5
        public async Task<ActionResult> AuthorDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            author author = await db.authors.FindAsync(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            return View(author);
        }

        // GET: authors/Create
        public ActionResult AuthorCreate()
        {
            return View();
        }

        // POST: authors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AuthorCreate([Bind(Include = "authorId,name,surname")] author author)
        {
            if (ModelState.IsValid)
            {
                db.authors.Add(author);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(author);
        }

        // GET: authors/Edit/5
        public async Task<ActionResult> AuthorEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            author author = await db.authors.FindAsync(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            return View(author);
        }

        // POST: authors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AuthorEdit([Bind(Include = "authorId,name,surname")] author author)
        {
            if (ModelState.IsValid)
            {
                db.Entry(author).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(author);
        }

        // GET: authors/Delete/5
        public async Task<ActionResult> AuthorDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            author author = await db.authors.FindAsync(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            return View(author);
        }

        // POST: authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            author author = await db.authors.FindAsync(id);
            db.authors.Remove(author);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        //--------------------------------------------Book------------------------//

        // GET: books
        public async Task<ActionResult> BookIndex()
        {
            var books = db.books.Include(b => b.author).Include(b => b.type);
            return View(await books.ToListAsync());
        }

        // GET: books/Details/5
        public async Task<ActionResult> BookDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            book book = await db.books.FindAsync(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // GET: books/Create
        public ActionResult BookCreate()
        {
            ViewBag.authorId = new SelectList(db.authors, "authorId", "name");
            ViewBag.typeId = new SelectList(db.types, "typeId", "name");
            return View();
        }

        // POST: books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BookCreate([Bind(Include = "bookId,name,pagecount,point,authorId,typeId")] book book)
        {
            if (ModelState.IsValid)
            {
                db.books.Add(book);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.authorId = new SelectList(db.authors, "authorId", "name", book.authorId);
            ViewBag.typeId = new SelectList(db.types, "typeId", "name", book.typeId);
            return View(book);
        }

        // GET: books/Edit/5
        public async Task<ActionResult> BookEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            book book = await db.books.FindAsync(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            ViewBag.authorId = new SelectList(db.authors, "authorId", "name", book.authorId);
            ViewBag.typeId = new SelectList(db.types, "typeId", "name", book.typeId);
            return View(book);
        }

        // POST: books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BookEdit([Bind(Include = "bookId,name,pagecount,point,authorId,typeId")] book book)
        {
            if (ModelState.IsValid)
            {
                db.Entry(book).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.authorId = new SelectList(db.authors, "authorId", "name", book.authorId);
            ViewBag.typeId = new SelectList(db.types, "typeId", "name", book.typeId);
            return View(book);
        }

        // GET: books/Delete/5
        public async Task<ActionResult> BookDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            book book = await db.books.FindAsync(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BookDeleteConfirmed(int id)
        {
            book book = await db.books.FindAsync(id);
            db.books.Remove(book);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        //--------------------------------------------------Borrow------------------//

        // GET: borrows
        public async Task<ActionResult> BorrowIndex()
        {
            var borrows = db.borrows.Include(b => b.book).Include(b => b.student);
            return View(await borrows.ToListAsync());
        }

        // GET: borrows/Details/5
        public async Task<ActionResult> BorrowDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            borrow borrow = await db.borrows.FindAsync(id);
            if (borrow == null)
            {
                return HttpNotFound();
            }
            return View(borrow);
        }

        // GET: borrows/Create
        public ActionResult BorrowCreate()
        {
            ViewBag.bookId = new SelectList(db.books, "bookId", "name");
            ViewBag.studentId = new SelectList(db.students, "studentId", "name");
            return View();
        }

        // POST: borrows/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BorrowCreate([Bind(Include = "borrowId,studentId,bookId,takenDate,broughtDate")] borrow borrow)
        {
            if (ModelState.IsValid)
            {
                db.borrows.Add(borrow);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.bookId = new SelectList(db.books, "bookId", "name", borrow.bookId);
            ViewBag.studentId = new SelectList(db.students, "studentId", "name", borrow.studentId);
            return View(borrow);
        }

        // GET: borrows/Edit/5
        public async Task<ActionResult> BorrowEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            borrow borrow = await db.borrows.FindAsync(id);
            if (borrow == null)
            {
                return HttpNotFound();
            }
            ViewBag.bookId = new SelectList(db.books, "bookId", "name", borrow.bookId);
            ViewBag.studentId = new SelectList(db.students, "studentId", "name", borrow.studentId);
            return View(borrow);
        }

        // POST: borrows/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BorrowEdit([Bind(Include = "borrowId,studentId,bookId,takenDate,broughtDate")] borrow borrow)
        {
            if (ModelState.IsValid)
            {
                db.Entry(borrow).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.bookId = new SelectList(db.books, "bookId", "name", borrow.bookId);
            ViewBag.studentId = new SelectList(db.students, "studentId", "name", borrow.studentId);
            return View(borrow);
        }

        // GET: borrows/Delete/5
        public async Task<ActionResult> BorrowDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            borrow borrow = await db.borrows.FindAsync(id);
            if (borrow == null)
            {
                return HttpNotFound();
            }
            return View(borrow);
        }

        // POST: borrows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BorrowDeleteConfirmed(int id)
        {
            borrow borrow = await db.borrows.FindAsync(id);
            db.borrows.Remove(borrow);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        //-------------------------------------------Students--------------------------------//

        // GET: students
        public async Task<ActionResult> StudentIndex()
        {
            return View(await db.students.ToListAsync());
        }

        // GET: students/Details/5
        public async Task<ActionResult> StudentDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            student student = await db.students.FindAsync(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: students/Create
        public ActionResult StudentCreate()
        {
            return View();
        }

        // POST: students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> StudentCreate([Bind(Include = "studentId,name,surname,birthdate,gender,class,point")] student student)
        {
            if (ModelState.IsValid)
            {
                db.students.Add(student);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(student);
        }

        // GET: students/Edit/5
        public async Task<ActionResult> StudentEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            student student = await db.students.FindAsync(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> StudentEdit([Bind(Include = "studentId,name,surname,birthdate,gender,class,point")] student student)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(student);
        }

        // GET: students/Delete/5
        public async Task<ActionResult> StudentDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            student student = await db.students.FindAsync(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> StudentDeleteConfirmed(int id)
        {
            student student = await db.students.FindAsync(id);
            db.students.Remove(student);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        //----------------------------Types-----------------------------------------//

        // GET: types
        public async Task<ActionResult> TypeIndex()
        {
            return View(await db.types.ToListAsync());
        }

        // GET: types/Details/5
        public async Task<ActionResult> TypeDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            type type = await db.types.FindAsync(id);
            if (type == null)
            {
                return HttpNotFound();
            }
            return View(type);
        }

        // GET: types/Create
        public ActionResult TypeCreate()
        {
            return View();
        }

        // POST: types/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TypeCreate([Bind(Include = "typeId,name")] type type)
        {
            if (ModelState.IsValid)
            {
                db.types.Add(type);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(type);
        }

        // GET: types/Edit/5
        public async Task<ActionResult> TypeEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            type type = await db.types.FindAsync(id);
            if (type == null)
            {
                return HttpNotFound();
            }
            return View(type);
        }

        // POST: types/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TypeEdit([Bind(Include = "typeId,name")] type type)
        {
            if (ModelState.IsValid)
            {
                db.Entry(type).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(type);
        }

        // GET: types/Delete/5
        public async Task<ActionResult> TypeDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            type type = await db.types.FindAsync(id);
            if (type == null)
            {
                return HttpNotFound();
            }
            return View(type);
        }

        // POST: types/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TypeDeleteConfirmed(int id)
        {
            type type = await db.types.FindAsync(id);
            db.types.Remove(type);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        //-------------------------Maintain Controller------------------------//

        public async Task<ActionResult> Maintain(int skipPage = 1, int skipPage1 = 1, int skipPage2 = 1)
        {
            var maintModel = new CombinedViewModel();

            var myAuthors = db.authors.Include(b => b.books).OrderBy(a => a.authorId);
            maintModel.Authors = await myAuthors.Skip((skipPage - 1) * size).Take(size).ToListAsync();
            ViewBag.skipPage = (int)Math.Ceiling((double)myAuthors.Count() / size);



            maintModel.Books = await db.books.Include(a => a.author).Include(ty => ty.type).Include(bo => bo.borrows).ToListAsync();


            maintModel.Students = await db.students.Include(bo => bo.borrows).ToListAsync();


            var myTypes = db.types.Include(b => b.books).OrderBy(t => t.typeId);
            maintModel.Types = await myTypes.Skip((skipPage - 1) * size).Take(size).ToListAsync();


            var myBorrows = db.borrows.Include(b => b.book).Include(st => st.student).OrderBy(br => br.borrowId);
            maintModel.Borrows = await db.borrows.Include(b => b.book).Include(s => s.student).ToListAsync();



            ViewBag.Page = skipPage;
            ViewBag.skipAuthor = (int)Math.Ceiling((double)myAuthors.Count() / size);
            ViewBag.skipType = (int)Math.Ceiling((double)myTypes.Count() / size);
            ViewBag.skipBorrows = (int)Math.Ceiling((double)myBorrows.Count() / size);


            return View(maintModel);




        }

        public ActionResult Reports()
        {
            ViewBag.Message = "Report page";

            return View();

        }



    }
}