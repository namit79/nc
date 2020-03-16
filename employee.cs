using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using eleven.Models;
using eleven.NewFolder;
using MySql.Data.MySqlClient;


namespace eleven
{
    public class employee
    {
        
        public appDb Db { get; }
       
        public employee(appDb db)
        {
            Db = db;
        }



        // querry execute
        public async Task<List<employees>> getUserInformation()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"select employee.employeeId, CONCAT(firstName , ' ' , middleName , ' ' , lastname) as Name,countryName as nationality,
CONCAT(addressLine1 , ',' , addressLine2 , ' ,' , cityName,',',stateName,',',pincode)as address,GROUP_CONCAT(contact_information.contactNumber) as `numbers`,
TIMESTAMPDIFF(YEAR,dateOfJoing,CURDATE()) as experienceyaer, TIMESTAMPDIFF( MONTH, dateOfJoing, now() ) % 12 as currentExperiencemonth,
    TIMESTAMPDIFF ( YEAR, birthDate, now() ) as year
    , TIMESTAMPDIFF( MONTH, birthDate, now() ) % 12 as month
    , ( TIMESTAMPDIFF( DAY, birthDate, now() ) % 30 ) as day
FROM employee,address,join_date,city,country,contact_information,state where employee.employeeId=address.addressId and
address.cityId=city.cityId and employee.employeeId=join_date.employeeId and employee.nationality=country.countryId and state.stateId=city.stateId
 and employee.employeeId=contact_information.employeeId group by
  employee.employeeId;";
            return await ReadAllEmployee(await cmd.ExecuteReaderAsync());
        }

        public async Task<List<employees>> getUserInformationById(int id)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"select * from  (select employee.employeeId, CONCAT(firstName , ' ' , middleName , ' ' , lastname) as Name,countryName as nationality,
CONCAT(addressLine1 , ',' , addressLine2 , ' ,' , cityName,',',stateName,',',pincode)as address,GROUP_CONCAT(contact_information.contactNumber) as `numbers`,
TIMESTAMPDIFF(YEAR,dateOfJoing,CURDATE()) as experienceyaer, TIMESTAMPDIFF( MONTH, dateOfJoing, now() ) % 12 as currentExperiencemonth,
    TIMESTAMPDIFF ( YEAR, birthDate, now() ) as year
    , TIMESTAMPDIFF( MONTH, birthDate, now() ) % 12 as month
    , ( TIMESTAMPDIFF( DAY, birthDate, now() ) % 30 ) as day
FROM employee,address,join_date,city,country,contact_information,state where employee.employeeId=address.addressId and
address.cityId=city.cityId and employee.employeeId=join_date.employeeId and employee.nationality=country.countryId and state.stateId=city.stateId
 and employee.employeeId=contact_information.employeeId and employee.employeeId=@id group by
  employee.employeeId ) as user ;";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@id",
                DbType = DbType.Int32,
                Value = id,
            });
            return await ReadAllEmployee(await cmd.ExecuteReaderAsync());
          
        }
        public async Task<List<employees>> getUserInformationByName(string name)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"select* from(select employee.employeeId , CONCAT(firstName,' ', middleName,' ', lastname) as Name , countryName as nationality,
CONCAT(addressLine1, ',', addressLine2, ' ,', cityName, ',', stateName, ',', pincode) as address,GROUP_CONCAT(contact_information.contactNumber) as `numbers`,
TIMESTAMPDIFF(YEAR, dateOfJoing, CURDATE()) as experienceyaer, TIMESTAMPDIFF(MONTH, dateOfJoing, now()) % 12 as currentExperiencemonth,
    TIMESTAMPDIFF(YEAR, birthDate, now()) as year
    , TIMESTAMPDIFF(MONTH, birthDate, now()) % 12 as month
    , (TIMESTAMPDIFF(DAY, birthDate, now()) % 30) as day
FROM employee, address, join_date, city, country, contact_information, state where employee.employeeId = address.addressId and
address.cityId = city.cityId and employee.employeeId = join_date.employeeId and employee.nationality = country.countryId and state.stateId = city.stateId
 and employee.employeeId = contact_information.employeeId group by
  employee.employeeId)as user WHERE Name LIKE '%" + @name + "%'";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@name",
                DbType = DbType.String,
                Value = name,
            });
            
            return await ReadAllEmployee(await cmd.ExecuteReaderAsync());

        }


        public async Task<List<employees>> UpdateUserInformationById(int id,string firstName, string addressLine1)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"update employee,address set employee.firstName=@firstName,address.addressLine1=@addressLine1 where employee.employeeId=address.employeeId and address.employeeId=@id;";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@firstName",
                DbType = DbType.String,
                Value = firstName,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@id",
                DbType = DbType.Int32,
                Value = id,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@addressLine1",
                DbType = DbType.String,
                Value = addressLine1,
            });
            return await ReadAllEmployee(await cmd.ExecuteReaderAsync());
        }
       

        //mapping 

        public async Task<List<employees>> ReadAllEmployee(DbDataReader reader)
        {
            var posts = new List<employees>();   
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    var post = new employees()
                    {
                        //Id = reader.GetString(0),
                        Name = reader.GetString(1),
                        nationality = reader.GetString(2),
                        Address = reader.GetString(3),
                        contactDetail = new contactInformation(reader.GetString(4)),
                        currentCompanyExp = reader.GetString(5)+ " years " + reader.GetString(6) + " months",
                        Age = new Models.dateOfBirth(reader.GetString(7) , reader.GetString(8) , reader.GetString(9)),
                       

                    };
                    posts.Add(post);
                }
            }
            return posts;
        }

       

    }
}
