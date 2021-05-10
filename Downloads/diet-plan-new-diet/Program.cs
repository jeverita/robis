using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;


namespace DietTrackor
{
    class Program
    {
        

        static void Main(string[] args)
        {

            bool exist = false;


            string connString = "server=localhost;port=3306;database=dietplan;user=newuser;password=password";
            MySqlConnection conn = new MySqlConnection(connString);
            conn.Open();

            string userName; 
            char gender = Convert.ToChar("F");
            int age = 0;
            int height = 0;
            int weight = 0;
            int workOutLevel = 0;
            int goalFatLoss = 0;
            int dietLength = 0;
            int caloriesConsumed = 0, coffeeConsumed = 0, caloriesBurnt = 0;
            double BMR , caloriesMaintenance , caloriesDeficitTarget = 0 , recommendedDailyCalories = 0;
            int currentDayCount=0, daysRemaining ;
            double caloriesLost = 0, caloriesLostTotal = 0, calorieDeficitAverage = 0, calorieDeficit = 0, fatLoss=0;
            double fatLossAverage = 0, fatLossSooFar = 0, fatLossExpected=0; 
            string x;
            BMR = 0;
            caloriesMaintenance = 0;

            Console.WriteLine("Enter your name");
            userName = Console.ReadLine();
            //Check if user already exists
            MySqlCommand cmd3 = new MySqlCommand(@"SELECT id FROM newuser WHERE userName = @userName", conn);
            cmd3.Parameters.AddWithValue("@userName", userName);
            
            uint? id = (uint?) cmd3.ExecuteScalar();

            exist = id != null;

             if (!exist)
                    {
                    Console.WriteLine("Enter your gender\n -Press M for male\n -Press F for female");
                    gender = Convert.ToChar(Console.ReadLine());
                    
                    Console.WriteLine("Enter your age");
                    age = Convert.ToInt32(Console.ReadLine());

                    Console.WriteLine("Enter your height (cm)");
                    height = Convert.ToInt32(Console.ReadLine());

                    Console.WriteLine("Enter your weight (kg)");
                    weight = Convert.ToInt32(Console.ReadLine());

                    Console.WriteLine("Enter your workout level\n -Press 1 for Light/No exercise\n -Press 2 for Light exercise\n -Press 3 for Moderate exercise\n -Press 4 for very active exercise\n -Press 5 for extra active exercise");
                    workOutLevel = Convert.ToInt32(Console.ReadLine());

                    Console.WriteLine("Enter your fat loss goal (kg)");
                    goalFatLoss = Convert.ToInt32(Console.ReadLine());

                    Console.WriteLine("Enter your diet length (days)");

                    dietLength = Convert.ToInt32(Console.ReadLine());
                       
            
                        if (gender == 'M')
                        {
                            BMR = (10 * weight) + (6.25 * height) - (5 * age) + 5;
                        }
                        else if (gender == 'F')
                        {

                            BMR = (10 * weight) + (6.25 * height) - (5 * age) - 161;
                        }
                
                        if (workOutLevel == 1)
                            caloriesMaintenance = BMR * 1.2;
                        else if (workOutLevel == 2)
                            caloriesMaintenance = BMR * 1.375;
                        else if (workOutLevel == 3)
                            caloriesMaintenance = BMR * 1.55;
                        else if (workOutLevel == 4)
                            caloriesMaintenance = BMR * 1.725;
                        else if (workOutLevel == 5)
                            caloriesMaintenance = BMR * 1.9;
                    caloriesDeficitTarget = ((goalFatLoss / 0.0128) * 100) / dietLength;
                    //Insert data in newuser and also create the individual id for username
                    MySqlCommand cmd = new MySqlCommand("INSERT INTO newUser(userName, age, gender, height, weight, workOutLevel, goalFatLoss, dietLength, id, BMR, caloriesMaintenance, caloriesDeficitTarget) VALUES(@userName, @age, @gender, @height, @weight, @workOutLevel, @goalFatLoss, @dietLength, LAST_INSERT_ID(), @BMR, @caloriesMaintenance, @caloriesDeficitTarget)", conn);
                    cmd.Parameters.AddWithValue("@userName", userName);
                    cmd.Parameters.AddWithValue("@age", age);
                    cmd.Parameters.AddWithValue("@gender", gender);
                    cmd.Parameters.AddWithValue("@height", height);
                    cmd.Parameters.AddWithValue("@weight", weight);
                    cmd.Parameters.AddWithValue("@workOutLevel", workOutLevel);
                    cmd.Parameters.AddWithValue("@goalFatLoss", goalFatLoss);
                    cmd.Parameters.AddWithValue("@dietLength", dietLength);
                    cmd.Parameters.AddWithValue("@BMR", BMR);
                    cmd.Parameters.AddWithValue("@caloriesMaintenance", caloriesMaintenance);
                    cmd.Parameters.AddWithValue("@caloriesDeficitTarget", caloriesDeficitTarget);
            


                    id = (uint?)cmd.ExecuteScalar();
                    
                   
           //If user already exists we offer these questions

                    }else 
                    {
                        do {
                            Console.WriteLine("Enter your calories consumed (kcal)");
                            caloriesConsumed = Convert.ToInt32(Console.ReadLine());

                            Console.WriteLine("Enter your coffee doses consumed");
                            coffeeConsumed = Convert.ToInt32(Console.ReadLine());

                            Console.WriteLine("Enter your calories burnt (kcal)");
                            caloriesBurnt = Convert.ToInt32(Console.ReadLine());
                            //Insert values into dietdays table
                            MySqlCommand cmd2 = new MySqlCommand("INSERT INTO dietDays(caloriesConsumed, coffeeConsumed, caloriesBurnt, newUser_id) VALUES(@caloriesConsumed, @coffeeConsumed, @caloriesBurnt, @newUser_id)", conn);
                            cmd2.Parameters.AddWithValue("@caloriesConsumed", caloriesConsumed);
                            cmd2.Parameters.AddWithValue("@coffeeConsumed", coffeeConsumed);
                            cmd2.Parameters.AddWithValue("@caloriesBurnt", caloriesBurnt);
                            cmd2.Parameters.AddWithValue("@newUser_id", id);

                            cmd2.ExecuteNonQuery();
                            // Count how many days of diet
                            MySqlCommand cmd5 = new MySqlCommand("SELECT COUNT(*) FROM dietdays WHERE newUser_id = '" + id + "'", conn);
                            currentDayCount = Convert.ToInt32(cmd5.ExecuteScalar());
                            
                            Console.WriteLine("Success!");
                            Console.Write($"If you want to add calories for day {currentDayCount+1} - press 'y', otherwise - press 'n'");
                            } 
                        while (Console.ReadLine().ToLower() == "y");
                    }
 
            Console.WriteLine("\nFor full diet report - Press 1, \nFor fat loss report - Press 2, \nFor calorie report - Press 3");
            int report = Convert.ToInt32(Console.ReadLine());
            //Read data from MySQL
            MySqlCommand cmd4 = new MySqlCommand("SELECT * FROM newuser WHERE userName = '" + userName + "'", conn);

            MySqlDataReader reader = cmd4.ExecuteReader();
            while(reader.Read())    
            {
                BMR = Convert.ToDouble(reader["BMR"].ToString());
                caloriesMaintenance =  Convert.ToDouble(reader["caloriesMaintenance"].ToString());
                caloriesDeficitTarget =  Convert.ToDouble(reader["caloriesDeficitTarget"].ToString());
                dietLength =  Convert.ToInt32(reader["dietLength"].ToString());
              }conn.Close(); // had to change connection closure to close the connection after the loop is done
            daysRemaining = dietLength - currentDayCount;
            calorieDeficit = caloriesMaintenance - caloriesConsumed - caloriesBurnt;

            caloriesLost = caloriesConsumed - calorieDeficit;
            recommendedDailyCalories = caloriesMaintenance - caloriesDeficitTarget;

            fatLoss = (calorieDeficit * 0.128)-(0.02 * coffeeConsumed);
            
            
            if (report == 1)
            {
                Console.WriteLine("\n****Diet Report of " + userName + "****");
                Console.WriteLine("\nBMR = {0}\nCalories Maintenance = {1} kcal\nRecommended Daily Calories = {2} kcal\nCurrent Day Count = Day {3}\nRemaining Days = {4} days", BMR, caloriesMaintenance, recommendedDailyCalories, currentDayCount, daysRemaining);
                Console.WriteLine("\nCalories Lost = {0} kcal\nCalories Deficit Target = {1} kcal\nTotal Calories Lost = {2} kcal\nAverage Calorie Deficit = {3} kcal\nCalorie Deficit = {4}  ", caloriesLost, caloriesDeficitTarget, caloriesLostTotal, calorieDeficitAverage, calorieDeficit);
                Console.WriteLine("\nFat Loss = {0} g\nFat Loss Average = {1} g \nFat Loss So Far = {2} g\nFat Loss Expected = {3} g", fatLoss, fatLossAverage, fatLossSooFar, fatLossExpected);
                Console.WriteLine("\n*******************************\nPress x to continue");
            }
            else if (report == 2)
            {
                Console.WriteLine("\n****Fat Loss Report of " + userName + "****");
                Console.WriteLine("\nFat Loss = {0} g\nFat Loss Average = {1} g \nFat Loss So Far = {2} g\nFat Loss Expected = {3} g", fatLoss, fatLossAverage, fatLossSooFar, fatLossExpected);
                Console.WriteLine("\n*******************************\nPress x to continue");
            }
            else if (report == 3)
            {
                Console.WriteLine("\n****Calorie Report of " + userName + "****");
                Console.WriteLine("\nBMR = {0}\nCalories Maintenance = {1} kcal\nRecommended Daily Calories = {2} kcal\nCurrent Day Count = Day {3}\nRemaining Days = {4} days", BMR, caloriesMaintenance, recommendedDailyCalories, currentDayCount, daysRemaining);
                Console.WriteLine("\nCalories Lost = {0} kcal\nCalories Deficit Target = {1} kcal\nTotal Calories Lost = {2} kcal\nAverage Calorie Deficit = {3} kcal\nCalorie Deficit = {4}  ", caloriesLost, caloriesDeficitTarget, caloriesLostTotal, calorieDeficitAverage, calorieDeficit);
                Console.WriteLine("\n*******************************\nPress x to continue");
            }
            x = Console.ReadLine();
           
        }
    }
}