# AdoCore
Structure that Support the use of Ado .Net in Asp net Core.

To include, follow the steps below:

- Download dependence from nuget with command 
  
  Install-Package Ado

- In Method ConfigureServices add this code to include provider of SQL SERVER:
  
  System.Data.Common.DbProviderFactories.AddSqlServerFactory();
  
- In file appsettings.json add the follow code:
  
  "ConnectionStrings": {
    "DefaultConnection": "YourConnectionStringHere"
  }
 
Done! Your project is configured to use AdoNet sctructure.

You could use this structure as follows:

using Ado;
using System.Linq;

//Without parameters
public List<Student> GetAllStudents(){
   
   using (var context = new DataContext())
   {
       return context.GetWhere<Student>("SELECT * FROM Student").ToList();
   }
}

//With parameters
public List<Student> GetAllStudentsInSchoolAndCourse(int idSchool, int idCourse){
   
   using (var context = new DataContext())
   {
       return context.GetWhere<Student>("SELECT * FROM Student Where idScholl = @0 and idCourse = @1", idScholl, idCourse).ToList();
   }
}

public int Include(Student student){
   
   using (var context = new DataContext())
   {
       var idStudent = context.ExecuteGetIdentity("insert into student (name, age) values (@0, @1)", student.Name, student.Age);
       
       //If you want commit now call method below else you could call later.
       context.Commit();
       
       return idStudent;  
   }
}

public void Update(Student student){
   
   using (var context = new DataContext())
   {
       //The second parameter in Function ExecutarComando is if commit or no. Pass true to commit.
       
       var rowsAffected = 
       context.ExecutarComando("update student set name = @1, age = @2 where idStudent = @0", true, student.IdStudent, student.Name, student.Age);
   }
}

public void Delete(int idStudent){
   
   using (var context = new DataContext())
   { 
       var rowsAffected = 
       context.ExecutarComando("delete student where idStudent = @0", true, idStudent);
   }
}

----------------------------- MULTIPLE CONNECTIONS -------------------------------------------

To use another connection string, you can do that: 

- Include this in appSettings.json:

"ConnectionStrings": {
    "DefaultConnection": "YourConnectionStringHere",
    "MyAnotherConnectionString": "YourAnotherConnectionStringHere",
  }

- And use :
  
  using (var context = new DataContext("MyAnotherConnectionString"))



  

