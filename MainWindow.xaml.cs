using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;
using System.IO;
using Assignment5;
using System.Data;
using System.Data.SqlClient;
using MessageBox = System.Windows.MessageBox;

namespace Assignment9

{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        string connString = Utility.GetConnectionString();

        DataTable dt;

        bool isRunning = false;



        int current_primary_key = -1;

        int baseFaceOption = 1;

        int hairPicOption = 1;
        int hairNumOfPhotos = 8;

        int eyesPicOption = 1;
        int eyesNumOfPhotos = 8;

        int nosePicOption = 1;
        int noseNumOfPhotos = 7;

        int mouthPicOption = 1;
        int mouthNumOfPhotos = 7;

        List<TabItem> pages = new List<TabItem>();

        HotKey BackHair = new(() => Change_Body_Part("Hair", false), true);
        HotKey ForwardHair = new(() => Change_Body_Part("Hair", true), true);
        HotKey BackEyes = new(() => Change_Body_Part("Eyes", false), true);
        HotKey ForwardEyes = new(() => Change_Body_Part("Eyes", true), true);
        HotKey BackNose = new(() => Change_Body_Part("Nose", false), true);
        HotKey ForwardNose = new(() => Change_Body_Part("Nose", true), true);
        HotKey BackMouth = new(() => Change_Body_Part("Mouth", false), true);
        HotKey ForwardMouth = new(() => Change_Body_Part("Mouth", true), true);
        HotKey Randomize = new(Randomize_Face, true);
        HotKey NewDarkSkin = new(() => New_Skin_Tone(2), true);
        HotKey NewLightSkin = new(() => New_Skin_Tone(1), true);
        HotKey Exit = new(Exit_App, true);
        HotKey Help = new(Help_Menu, true);

        Editer editer = new();

        public MainWindow()
        {


            InitializeComponent();
            OccupationDropdown.Items.Insert(0, "--Select One--");
            OccupationDropdown.SelectedIndex = 0;
            HobbyDropdown.Items.Insert(0, "--Select One--");
            HobbyDropdown.SelectedIndex = 0;
            FillOccupation();
            FillHobby();
            FillDataGrid();
            isRunning = true;

            DataContext = new
            {
                backHair = BackHair,
                forwardHair = ForwardHair,
                backEyes = BackEyes,
                forwardEyes = ForwardEyes,
                backNose = BackNose,
                forwardNose = ForwardNose,
                backMouth = BackMouth,
                forwardMouth = ForwardMouth,
                randomize = Randomize,
                newDarkSkin = NewDarkSkin,
                newLightSkin = NewLightSkin,
                help = Help,
                exit = Exit
            };


            pages.Add(faceChanger);
            pages.Add(finish);

            InputBindings.Add(new KeyBinding(BackHair, new KeyGesture(Key.F1, ModifierKeys.None)));
            InputBindings.Add(new KeyBinding(ForwardHair, new KeyGesture(Key.F2, ModifierKeys.None)));

            InputBindings.Add(new KeyBinding(BackEyes, new KeyGesture(Key.F3, ModifierKeys.None)));
            InputBindings.Add(new KeyBinding(ForwardEyes, new KeyGesture(Key.F4, ModifierKeys.None)));

            InputBindings.Add(new KeyBinding(BackNose, new KeyGesture(Key.F5, ModifierKeys.None)));
            InputBindings.Add(new KeyBinding(ForwardNose, new KeyGesture(Key.F6, ModifierKeys.None)));

            InputBindings.Add(new KeyBinding(BackMouth, new KeyGesture(Key.F7, ModifierKeys.None)));
            InputBindings.Add(new KeyBinding(ForwardMouth, new KeyGesture(Key.F8, ModifierKeys.None)));

            InputBindings.Add(new KeyBinding(Randomize, new KeyGesture(Key.R, ModifierKeys.Control)));

            InputBindings.Add(new KeyBinding(NewDarkSkin, new KeyGesture(Key.D, ModifierKeys.Control)));
            InputBindings.Add(new KeyBinding(NewLightSkin, new KeyGesture(Key.L, ModifierKeys.Control)));

            InputBindings.Add(new KeyBinding(Help, new KeyGesture(Key.I, ModifierKeys.Control)));

            InputBindings.Add(new KeyBinding(Exit, new KeyGesture(Key.Q, ModifierKeys.Control)));

        }

        private void FillOccupation()
        {
            // Old school connection
            SqlConnection conn = new SqlConnection(connString);

            // old school insert statement...note Trace output should show format of SQL Insert command
            String cmd_Text_Details = "SELECT * FROM occupation;";
            Trace.Write(cmd_Text_Details);

            List<String> list = new List<String>();
            try
            {
                
                SqlCommand command = new SqlCommand(cmdText: cmd_Text_Details, connection: conn);
                command.Connection.Open();


                using (var reader = command.ExecuteReader())
                    while (reader.Read())
                    {

                        list.Add(reader.GetString(reader.GetOrdinal("name")));

                    }
                

                list.ForEach(item =>
                {
                    OccupationDropdown.Items.Add(item);
                });
            }
            catch (Exception e){ System.Windows.MessageBox.Show(e + " "); }


            
            finally { conn.Close(); }

        }

        private void FillHobby()
        {
            // Old school connection
            SqlConnection conn = new SqlConnection(connString);

            // old school insert statement...note Trace output should show format of SQL Insert command
            String cmd_Text_Details = "SELECT * FROM hobby;";
            Trace.Write(cmd_Text_Details);

            List<String> list = new List<String>();
            try
            {
                SqlCommand command = new SqlCommand(cmdText: cmd_Text_Details, connection: conn);
                command.Connection.Open();
                using (var reader = command.ExecuteReader())
                    while (reader.Read())
                    {

                        list.Add(reader.GetString(reader.GetOrdinal("name")));

                    }
                list.ForEach(item =>
                {
                   HobbyDropdown.Items.Add(item);
                });
            }
            catch (Exception e) { System.Windows.MessageBox.Show(e + " "); }
            finally { conn.Close(); }
        }

        private void FillDataGrid()
        {
            string CmdString = string.Empty;
            using (SqlConnection con = new SqlConnection(connString))
            {
                CmdString = "SELECT " +
                    "person.Id as 'Person ID'," +
                    "person.fName as 'First Name'," +
                    "person.lName as 'Last Name'," +
                    "person.city as 'City'," +
                    "face.baseFace as 'Base Face Option'," +
                    "face.hair as 'Hair Option'," +
                    "face.eyes as 'Eyes Option'," +
                    "face.nose as 'Nose Option'," +
                    "face.mouth as 'Mouth Option'," +
                    "occupation.name as 'Occupation'," +
                    "hobby.name as 'Hobby'" +
                    "FROM person " +
                    "INNER JOIN face " +
                    "ON (person.id = face.personId) " +
                    "INNER JOIN occupation " +
                    "ON (person.occupationId = occupation.Id) " +
                    "INNER JOIN hobby " +
                    "ON (person.hobbyId = hobby.Id);";
                SqlCommand cmd = new SqlCommand(CmdString, con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                dt = new DataTable("dataReadOut");
                sda.Fill(dt);
                dataReadOut.ItemsSource = dt.DefaultView;
            }
        }

        private void SearchDatabase(string query)
        {
            string CmdString = string.Empty;
            using (SqlConnection con = new SqlConnection(connString))
            {
                CmdString = "SELECT " +
                    "person.Id as 'Person ID'," +
                    "person.fName as 'First Name'," +
                    "person.lName as 'Last Name'," +
                    "person.city as 'City'," +
                    "face.baseFace as 'Base Face Option'," +
                    "face.hair as 'Hair Option'," +
                    "face.eyes as 'Eyes Option'," +
                    "face.nose as 'Nose Option'," +
                    "face.mouth as 'Mouth Option'," +
                    "occupation.name as 'Occupation'," +
                    "hobby.name as 'Hobby'" +
                    "FROM person " +
                    "INNER JOIN face " +
                    "ON (person.id = face.personId) " +
                    "INNER JOIN occupation " +
                    "ON (person.occupationId = occupation.Id) " +
                    "INNER JOIN hobby " +
                    "ON (person.hobbyId = hobby.Id) " +
                "WHERE person.lName = '" + query + "';";
                SqlCommand cmd = new SqlCommand(CmdString, con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                dt = new DataTable("dataReadOut");
                sda.Fill(dt);
                dataReadOut.ItemsSource = dt.DefaultView;
            }
        }

        // Add a person record with these attributes...SQL INSERT command
        private void addPerson(String fn, String ln, String address, int occupation, int hobby, int baseFace, int hair, int eyes, int nose, int mouth)
        {
            // Old school connection
            SqlConnection conn = new SqlConnection(connString);
            int personExistId = 0;
            String cmd_Select_User = "SELECT Id from person where fName = '" + fn + "' AND lName = '" + ln + "' AND city = '" + address + "';";
            Trace.Write(cmd_Select_User);
            try
            {
                SqlCommand command = new SqlCommand(cmdText: cmd_Select_User, connection: conn);
                command.Connection.Open();
                if(command.ExecuteScalar() != null)
                {
                    personExistId = (int)command.ExecuteScalar();
                }
            }
            catch { System.Windows.MessageBox.Show("DB Select Exception"); }

            if(personExistId == 0)
            {
                // old school insert statement...note Trace output should show format of SQL Insert command
                String cmd_Text_Details = "INSERT INTO person(fName, lName, city, occupationId, hobbyId)  VALUES('" + fn + "', '" + ln + "', '" + address + "', '" + occupation + "', '" + hobby + "');";
                Trace.Write(cmd_Text_Details);
                // DB insert in try-catch
                try
                {
                    // Example of C# named parameters...a good idea for important library calls
                    SqlCommand command = new SqlCommand(cmdText: cmd_Text_Details, connection: conn);
                    command.ExecuteNonQuery();  //does the actual insert statement

                }
                catch { 
                    System.Windows.MessageBox.Show("DB Add Exception");
                    conn.Close();
                }

                int personId = 0;

                cmd_Select_User = "SELECT Id from person where fName = '" + fn + "' AND lName = '" + ln + "' AND city = '" + address + "';";
                Trace.Write(cmd_Select_User);
                try
                {
                    SqlCommand command = new SqlCommand(cmdText: cmd_Select_User, connection: conn);

                    personId = (int)command.ExecuteScalar();
                }
                catch { System.Windows.MessageBox.Show("DB Select Exception");
                    conn.Close();
                }
                String cmd_Text_Face = "INSERT INTO face(personID, baseFace, hair, eyes, nose, mouth)  VALUES('" + personId + "', " + baseFace + ", " + hair + ", " + eyes + ", " + nose + ", " + mouth + ");";
                try
                {
                    // Example of C# named parameters...a good idea for important library calls
                    SqlCommand command = new SqlCommand(cmdText: cmd_Text_Face, connection: conn);

                    command.ExecuteNonQuery();  //does the actual insert statement
                }
                catch { System.Windows.MessageBox.Show("DB Add Exception");
                    conn.Close();
                }
            }  else
            {
                System.Windows.MessageBox.Show("User already exists. Please enter a different first name, last name or city");
            }
            conn.Close();
        }

        public static void Change_Body_Part(string part, bool isForward)
        {
            MainWindow thisWindow = ((MainWindow)System.Windows.Application.Current.MainWindow);
            Editer thisEditer = thisWindow.editer;

            if (part == "Hair")
            {
                if (!isForward)
                {
                    thisEditer.Previous_Image(thisWindow.Hair, thisWindow.HairResult, part.ToLower(), ref thisWindow.hairPicOption, thisWindow.hairNumOfPhotos);
                }
                else
                {
                    
                    thisEditer.Next_Image(thisWindow.Hair, thisWindow.HairResult, part.ToLower(), ref thisWindow.hairPicOption, thisWindow.hairNumOfPhotos);

                }
                Update_Label(part, thisWindow.HairLabel, thisWindow.hairPicOption);
            }

            if (part == "Eyes")
            {
                if (!isForward)
                {
                    thisEditer.Previous_Image(thisWindow.Eyes, thisWindow.EyesResult, part.ToLower(), ref thisWindow.eyesPicOption, thisWindow.eyesNumOfPhotos);

                }
                else
                {

                    thisEditer.Next_Image(thisWindow.Eyes, thisWindow.EyesResult, part.ToLower(), ref thisWindow.eyesPicOption, thisWindow.eyesNumOfPhotos);

                }
                Update_Label(part, thisWindow.EyesLabel, thisWindow.eyesPicOption);
            }

            if (part == "Nose")
            {
                if (!isForward)
                {
                    thisEditer.Previous_Image(thisWindow.Nose, thisWindow.NoseResult, part.ToLower(), ref thisWindow.nosePicOption, thisWindow.noseNumOfPhotos);
                }
                else
                {
                    thisEditer.Next_Image(thisWindow.Nose, thisWindow.NoseResult, part.ToLower(), ref thisWindow.nosePicOption, thisWindow.noseNumOfPhotos);
                }
                Update_Label(part, thisWindow.NoseLabel, thisWindow.nosePicOption);
            }

            if (part == "Mouth")
            {
                if (!isForward)
                {
                   thisEditer.Previous_Image(thisWindow.Mouth, thisWindow.MouthResult, part.ToLower(), ref thisWindow.mouthPicOption, thisWindow.mouthNumOfPhotos);
                }
                else
                {
                   thisEditer.Next_Image(thisWindow.Mouth, thisWindow.MouthResult, part.ToLower(), ref thisWindow.mouthPicOption, thisWindow.mouthNumOfPhotos);
                }
                Update_Label(part, thisWindow.MouthLabel, thisWindow.mouthPicOption);
            }
        }
        

        public static void Randomize_Face()
        {
            MainWindow thisWindow = ((MainWindow)System.Windows.Application.Current.MainWindow);
            Editer thisEditer = thisWindow.editer;

            if (thisWindow.HairCheckbox.IsChecked == true)
            {
                thisEditer.Random_Image(thisWindow.Hair, thisWindow.HairResult, "hair", ref thisWindow.hairPicOption, thisWindow.hairNumOfPhotos);
                Update_Label("Hair", thisWindow.HairLabel, thisWindow.hairPicOption);
            }
            if (thisWindow.EyesCheckbox.IsChecked == true)
            {
                thisEditer.Random_Image(thisWindow.Eyes, thisWindow.EyesResult, "eyes", ref thisWindow.eyesPicOption, thisWindow.eyesNumOfPhotos);
                Update_Label("Eyes", thisWindow.EyesLabel, thisWindow.eyesPicOption);
            }
            if (thisWindow.NoseCheckbox.IsChecked == true)
            {
                thisEditer.Random_Image(thisWindow.Nose, thisWindow.NoseResult, "nose", ref thisWindow.nosePicOption, thisWindow.noseNumOfPhotos);
                Update_Label("Nose", thisWindow.NoseLabel, thisWindow.nosePicOption);
            }
            if (thisWindow.MouthCheckbox.IsChecked == true)
            {
                thisEditer.Random_Image(thisWindow.Mouth, thisWindow.MouthResult, "mouth", ref thisWindow.mouthPicOption, thisWindow.mouthNumOfPhotos);
                Update_Label("Mouth", thisWindow.MouthLabel, thisWindow.mouthPicOption);
            }
        }

        public static void Update_Label(string part, TextBlock label, int index)
        {
            label.Text = part + " " + (index);
        }

        public static void New_Skin_Tone(int skinOption)
        {
            MainWindow thisWindow = ((MainWindow)System.Windows.Application.Current.MainWindow);
            thisWindow.baseFaceOption = skinOption;
            Editer thisEditer = thisWindow.editer;
            thisEditer.Set_Base_Face(skinOption, thisWindow.BaseFace);
            thisEditer.Set_Base_Face(skinOption, thisWindow.BaseFaceResult);
        }

        public static void Help_Menu()
        {
            HelpNavigator Nav_by_Topic = HelpNavigator.Topic;
            System.Windows.Forms.Help.ShowHelp(null, "FaceChanger.chm", Nav_by_Topic, "About.htm");
        }

        public static void Exit_App()
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HelpNavigator Nav_by_Topic = HelpNavigator.Topic;
            System.Windows.Forms.Help.ShowHelp(null, "FaceChanger.chm", Nav_by_Topic, "About.htm");
        }


        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            String fn = fNameSpace.Text;
            String ln = lNameSpace.Text;
            String a = citySpace.Text;
            int occupation = OccupationDropdown.SelectedIndex;
            int hobby = HobbyDropdown.SelectedIndex;
            int baseFace = baseFaceOption;
            int hair = hairPicOption;
            int eyes = eyesPicOption;
            int nose = nosePicOption;
            int mouth = mouthPicOption;


            // Add this record if values not empty
            if (fn != "" && ln != "" && a != "" && occupation > 0 && hobby > 0)
            {
                this.addPerson(fn, ln, a, occupation, hobby, baseFace, hair, eyes, nose, mouth);  // old school SQL-over-the-connection                                                            //this.addPerson1(fn, ln, city);  // Stored procedure
            }
            else
            {
                System.Windows.MessageBox.Show("A field is empty...enter all fields");
            }

            // Update changes to the grid
            FillDataGrid();
        }

        private void Previous_Page(object sender, RoutedEventArgs e)
        {
            int newActivePageIndex = Page_isSelected() - 1;
            if (newActivePageIndex < 0)
            {
                newActivePageIndex = 0;
            }

            pages[newActivePageIndex].IsSelected = true;

        }

        private void Next_Page(object sender, RoutedEventArgs e)
        {
            int newActivePageIndex = Page_isSelected() + 1;
            if (newActivePageIndex >= pages.Count)
            {
                newActivePageIndex = pages.Count - 1;
            }

            pages[newActivePageIndex].IsSelected = true;
        }


        private int Page_isSelected()
        {
            int index = 0;

            foreach (TabItem page in pages)
            {
                if (page.IsSelected)
                {
                    return index;
                }
                index++;
            }

            return 0;
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            current_primary_key = -1;
            fNameSpace.Text = string.Empty;
            lNameSpace.Text = string.Empty;
            citySpace.Text = string.Empty;
            OccupationDropdown.SelectedIndex = 0;
            HobbyDropdown.SelectedIndex = 0;
            hairPicOption = 1;
            eyesPicOption = 1;
            nosePicOption = 1;
            mouthPicOption = 1;
            editer.Update_Face("hair", Hair, hairPicOption);
            Update_Label("Hair", HairLabel, hairPicOption);

            editer.Update_Face("eyes", Eyes, eyesPicOption);
            Update_Label("Eyes", EyesLabel, eyesPicOption);

            editer.Update_Face("nose", Nose, nosePicOption);
            Update_Label("Nose", NoseLabel, nosePicOption);

            editer.Update_Face("mouth", Mouth, mouthPicOption);
            Update_Label("Mouth", MouthLabel, mouthPicOption);
            pages[0].IsSelected = true;

        }

        private void upPerson(int pkey, String fn, String ln, String city, int occupation, int hobby, int baseFace, int hair, int eyes, int nose, int mouth)
        {
            System.Windows.MessageBox.Show("" + hairPicOption);
            // Old school connection
            SqlConnection conn = new SqlConnection(connString);

            // old school insert statement...note Trace output should show format of SQL Insert command
            String cmd_Text =
                "UPDATE Person SET fname = '" + fn +
                "', lname = '" + ln +
                "', city = '" + city +
                "', occupationId = '" + occupation +
                "', hobbyId = '" + hobby +
                "'  WHERE Id = " + pkey + ";";
            Trace.Write(cmd_Text);

            // DB insert in try-catch
            try
            {
                // Example of C# named parameters...a good idea for important library calls
                SqlCommand command = new SqlCommand(cmdText: cmd_Text, connection: conn);
                command.Connection.Open();
                command.ExecuteNonQuery();  //does the actual insert statement
            }
            catch { MessageBox.Show("DB Update Exception"); }

            cmd_Text =
                "UPDATE face SET baseFace = " + baseFace +
                ", hair = " + hair +
                ", eyes = " + eyes +
                ", nose = " + nose +
                ", mouth = " + mouth +
                "  WHERE personId = " + pkey + ";";
            Trace.Write(cmd_Text);

            // DB insert in try-catch
            try
            {
                // Example of C# named parameters...a good idea for important library calls
                SqlCommand command = new SqlCommand(cmdText: cmd_Text, connection: conn);
                command.ExecuteNonQuery();  //does the actual insert statement
            }
            catch (Exception e) { MessageBox.Show("DB Update Exception" + e); }

            finally { conn.Close(); }

        }

        private void DlPerson(int pkey)
        {
            // Old school connection
            SqlConnection conn = new SqlConnection(connString);

            // old school insert statement...note Trace output should show format of SQL Insert command
            String cmd_Text = "DELETE FROM face WHERE personId = " + pkey + ";";
            Trace.Write(cmd_Text);

            // DB insert in try-catch
            try
            {
                // Example of C# named parameters...a good idea for important library calls
                SqlCommand command = new SqlCommand(cmdText: cmd_Text, connection: conn);
                command.Connection.Open();
                command.ExecuteNonQuery();  //does the actual insert statement
            }
            catch { MessageBox.Show("DB Delete Exception Face"); }

            String cmd_Delete_Person = "DELETE FROM person WHERE Id = " + pkey + ";";
            Trace.Write(cmd_Delete_Person);

            // DB insert in try-catch
            try
            {
                // Example of C# named parameters...a good idea for important library calls
                SqlCommand command = new SqlCommand(cmdText: cmd_Delete_Person, connection: conn);
                command.ExecuteNonQuery();  //does the actual insert statement
            }
            catch { MessageBox.Show("DB Delete Exception Person"); }
            finally { conn.Close(); }
        }

        private void dataReadOut_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataReadOut.SelectedItem != null && isRunning && dataReadOut.SelectedItem as DataRowView != null)
            {
                // When we get here after deleting a row, we can't get the current row
                try
                {
                    // fetch the columns from the selected row
                    current_primary_key = (int)(dataReadOut.SelectedItem as DataRowView).Row["Person ID"];
                    fNameSpace.Text = (string)(dataReadOut.SelectedItem as DataRowView).Row["First Name"];
                    lNameSpace.Text = (string)(dataReadOut.SelectedItem as DataRowView).Row["Last Name"];
                    citySpace.Text = (string)(dataReadOut.SelectedItem as DataRowView).Row["City"];
                    OccupationDropdown.SelectedIndex = OccupationDropdown.Items.IndexOf((string)(dataReadOut.SelectedItem as DataRowView).Row["Occupation"]);
                    HobbyDropdown.SelectedIndex = HobbyDropdown.Items.IndexOf((string)(dataReadOut.SelectedItem as DataRowView).Row["Hobby"]);

                    baseFaceOption = (int)(dataReadOut.SelectedItem as DataRowView).Row["Base Face Option"];
                    hairPicOption = (int)(dataReadOut.SelectedItem as DataRowView).Row["Hair Option"];
                    eyesPicOption = (int)(dataReadOut.SelectedItem as DataRowView).Row["Eyes Option"];
                    nosePicOption = (int)(dataReadOut.SelectedItem as DataRowView).Row["Nose Option"];
                    mouthPicOption = (int)(dataReadOut.SelectedItem as DataRowView).Row["Mouth Option"];


                    editer.Set_Base_Face(baseFaceOption, BaseFaceResult);
                    editer.Update_Face("hair", HairResult, hairPicOption);
                    editer.Update_Face("eyes", EyesResult, eyesPicOption);
                    editer.Update_Face("nose", NoseResult, nosePicOption);
                    editer.Update_Face("mouth", MouthResult, mouthPicOption);

                    editer.Set_Base_Face(baseFaceOption, BaseFace);
                    editer.Update_Face("hair", Hair, hairPicOption);
                    editer.Update_Face("eyes", Eyes, eyesPicOption);
                    editer.Update_Face("nose", Nose, nosePicOption);
                    editer.Update_Face("mouth", Mouth, mouthPicOption);

                    Update_Label("Hair", HairLabel, hairPicOption);
                    Update_Label("Eyes", EyesLabel, eyesPicOption);
                    Update_Label("Nose", NoseLabel, nosePicOption);
                    Update_Label("Mouth", MouthLabel, mouthPicOption);


                    Trace.WriteLine("Selected = " + current_primary_key + fNameSpace.Text + lNameSpace.Text);
                }
                catch //(Exception except)
                {
                    // MessageBox.Show("Selection error: " + except);
                    // If deleting row, get exception trying to get it's data
                    Trace.WriteLine("No Row (deleted?)...default record used");
                    current_primary_key = -1;
                    fNameSpace.Text = "";
                    lNameSpace.Text = "";
                    citySpace.Text = "";
                    OccupationDropdown.SelectedIndex = 0;
                    HobbyDropdown.SelectedIndex = 0;

                    hairPicOption = 1;
                    eyesPicOption = 1;
                    nosePicOption = 1;
                    mouthPicOption = 1;

                    editer.Update_Face("hair", HairResult, hairPicOption);
                    editer.Update_Face("eyes", EyesResult, eyesPicOption);
                    editer.Update_Face("nose", NoseResult, nosePicOption);
                    editer.Update_Face("mouth", MouthResult, mouthPicOption);

                    editer.Update_Face("hair", Hair, hairPicOption);
                    editer.Update_Face("eyes", Eyes, eyesPicOption);
                    editer.Update_Face("nose", Nose, nosePicOption);
                    editer.Update_Face("mouth", Mouth, mouthPicOption);

                    Update_Label("Hair", HairLabel, hairPicOption);
                    Update_Label("Eyes", EyesLabel, eyesPicOption);
                    Update_Label("Nose", NoseLabel, nosePicOption);
                    Update_Label("Mouth", MouthLabel, mouthPicOption);
                }
            }
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (current_primary_key > -1)
            {
                upPerson(current_primary_key, fNameSpace.Text, lNameSpace.Text, citySpace.Text, OccupationDropdown.SelectedIndex, HobbyDropdown.SelectedIndex, baseFaceOption, hairPicOption, eyesPicOption, nosePicOption, mouthPicOption);

                // Update changes to the grid
                FillDataGrid();
            } else
            {
                MessageBox.Show("Please select a user to edit!");
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (dataReadOut.SelectedItem != null)
            {
                DlPerson(current_primary_key);
            }

            FillDataGrid();
        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            string searchQuery = searchBar.Text;

            SearchDatabase(searchQuery);
        }

        private void defaultView_Click(object sender, RoutedEventArgs e)
        {
            FillDataGrid();
        }
    }
}
