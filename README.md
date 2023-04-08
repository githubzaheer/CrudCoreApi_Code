# ASP.NET CRUD Applica􀆟on with REST API and CI/CD Deployment to AWS Cloud using AWS Elasic Beanstalk
Git Repo - https://github.com/githubzaheer/CrudCoreApi_Code

Created a new Asp.net core api project with the following steps with code first apporach.
1. Select template
![image](https://user-images.githubusercontent.com/129961356/230733241-56880d6c-e141-47a8-9dd1-d734df7edc3c.png)
2. Add the name of the project
![image](https://user-images.githubusercontent.com/129961356/230733327-e9ec40de-6b22-4b8c-ac2e-6ff7c38af5db.png)
3. Choose required .net framework
![image](https://user-images.githubusercontent.com/129961356/230733404-9f734e60-8999-4c0c-ae25-b900c88a8871.png)
4. Click Create
5. Project Created successully 
6. Install the following required packages
![image](https://user-images.githubusercontent.com/129961356/230733663-0c724359-7d8d-4adc-9efe-0769eec7ea2d.png)
7. Add Product model class
![image](https://user-images.githubusercontent.com/129961356/230733771-6cec97f9-2420-40ed-b2b1-01a8ac46820d.png)
8. Add User Login class
![image](https://user-images.githubusercontent.com/129961356/230733816-010bf653-1c59-4763-bd77-409aa4c9ca82.png)
9. Add Class for constant users we can add table in database as well for users but this time i am using constants because of some reasons i will explain later in this fie.
![image](https://user-images.githubusercontent.com/129961356/230733902-2e371561-4cf4-40c3-b198-db2638d14c24.png)
10. Add connection string in appsettings.json
"ConnectionStrings": {
    "DefaultConnection": "Server=(local);Database=Crud_CoreAppDB;Trusted_Connection=True;MultipleActiveResultSets=true; TrustServerCertificate=True"
    }
11. Add Services to the container in Program.cs
  builder.Services.AddDbContext<ApplicationDbContext>
    (options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
12. Run the following command in Package Manager Console
  Add-Migration "AnyName"
  update-database
13. After executing the commands successfully a Migration folder will be create automatically.
  ![image](https://user-images.githubusercontent.com/129961356/230734323-d1f12327-e222-431e-91bd-d89996139c5f.png)
14. Add a new Api controller using entity framework and select required entity.
  ![image](https://user-images.githubusercontent.com/129961356/230734388-4366ab83-5309-409e-b76d-265fb4bec1e3.png)
15. Logger Implementaion 
  ![image](https://user-images.githubusercontent.com/129961356/230734533-2ad5fe0d-aa45-4b3f-8854-b30fa7c4bb96.png)
and add in programe.css
  builder.Services.AddLogging(logging =>
  {
    logging.ClearProviders(); // optional (clear providers already added)
    logging.AddFile("Logs/Log-{Date}.txt");
  });
16. Run the project to test api calling you can see api running successfully but there is no data in databse so the response is empty.
  ![image](https://user-images.githubusercontent.com/129961356/230734928-e1c79ce1-e32d-4251-870d-d231be12cfd6.png)
17. Now Lets implement the JWT token generation and applying the authorization. 
    Add in Appsettings.json
  "Jwt": {
    "Key": "DhftOS5uphK3vmCJQrexST1RsyjZBjXWRgJMFPU4",
    "Issuer": "http://localhost:34355",
    "Audience": "http://localhost:34355"
  }
  then add in programe.cs
  builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
     .AddJwtBearer(options => {
         options.TokenValidationParameters = new TokenValidationParameters
         {
             ValidateIssuer = true,
             ValidateAudience = true,
             ValidateLifetime = true,
             ValidateIssuerSigningKey = true,
             ValidIssuer = builder.Configuration["Jwt:Issuer"],
             ValidAudience = builder.Configuration["Jwt:Audience"],
             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
         };
     });
  app.UseAuthentication();
18. Create Login controller
   public class LoginController : ControllerBase
    {
        private IConfiguration _config;

        public LoginController(IConfiguration config)
        {
            _config = config;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] UserLogin userLogin)
        {
            var user = Authenticate(userLogin);

            if (user != null)
            {
                var token = Generate(user);
                return Ok(token);
            }

            return NotFound("User not found");
        }

        private string Generate(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])); //
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Email, user.EmailAddress),
                new Claim(ClaimTypes.GivenName, user.GivenName),
                new Claim(ClaimTypes.Surname, user.Surname),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Audience"],
              claims,
              expires: DateTime.Now.AddMinutes(15),
              signingCredentials: credentials);

           

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private User Authenticate(UserLogin userLogin)
        {
            var currentUser = UserConstants.Users.FirstOrDefault(o => o.Username.ToLower() == userLogin.Username.ToLower() && o.Password == userLogin.Password);

            if (currentUser != null)
            {
                return currentUser;
            }

            return null;
        }
    }
19. Run the Project again and call the login api. Api will generate tocken and now lets implement the Authorization
20. ![image](https://user-images.githubusercontent.com/129961356/230735260-92125be8-43bb-41d7-ac22-3f6293b96550.png)
21. now run the project again and see the GetProduct api will not call directly it will return Unathorized in response.
![image](https://user-images.githubusercontent.com/129961356/230735361-97995d9a-c2c2-421f-ad99-48df227aa3e0.png)
22. Now call the Login Api to generate the token. 
  ![image](https://user-images.githubusercontent.com/129961356/230735408-b7d035ed-bce1-4028-8bd3-21acc3603e07.png)
23. lets copy the token and paste it in Authorization Type.
  ![image](https://user-images.githubusercontent.com/129961356/230735457-589b2f80-6204-4f63-98ae-dd1c382041af.png)
Api will called successfully.
  
  
24. Lets Implement the Gitrepo creation.. 
  got to github account then go to repositories then create new then fill the required info also select to add redme file and mark as public privacy.
25. Install Git bash.
26. Open the code folder and right click and selct git bash option 
27. git bash interface will open then type command 
  git init
  git status 
  git add . 
  git remote add origin "GitrepositoryURL"
  git commit --m "comments"
  git push origin master --force
28. refresh the github page and you will see the files are uploaded.
  add two more files on github repo
  buildspec.yml
  ![image](https://user-images.githubusercontent.com/129961356/230736468-62d328b5-731b-489e-a442-ff3cf7d3b61e.png)
  and Procfile.
  ![image](https://user-images.githubusercontent.com/129961356/230736490-f18f3664-e2df-4c28-8c27-662f696b2884.png)

  
29. Lets Create the AWS account if not exist
  Log into your AWS account and navigate to AWS Elastic Beanstalk. Here, click on Create Application. Here, just give a nice name for your web application.
  ![image](https://user-images.githubusercontent.com/129961356/230735877-5dd08cfe-3532-45d6-9d8b-5b0900658533.png)
30. Fill the required information and click create 
  ![image](https://user-images.githubusercontent.com/129961356/230735933-722ac3e6-9283-4321-a486-a0e47021e809.png)
31. it will take time to create wait until to show this screen 
 ![image](https://user-images.githubusercontent.com/129961356/230735969-fd10553f-8168-4950-814b-0ff50c47d4d5.png)
32. Here is what you will be seeing by default once you access the provided URL.
  
  
33. Creating an AWS Codepipeline
  Open up AWS CodePipeline. Here, click on Create Pipeline. Here, give a nice name for your new pipeline. Leave all other settings to default and click next.
  ![image](https://user-images.githubusercontent.com/129961356/230736073-abc4f1b1-90d8-408f-8993-795a31f1d619.png)
34. Next comes the interesting part where we connect to GitHub and link our repository to the pipeline. Make sure to select GitHub version 2. Next, click on Connect to GitHub (if you are doing this for the first time). You will have to give permission to AWS to access your GitHub profile.
![image](https://user-images.githubusercontent.com/129961356/230736129-369ef9ef-7d6d-441b-af2d-2a0dec57f35f.png)
35. You will get a prompt from GitHub to allow access. For now, I have given access to all repositories. You can choose specific repositories as well.
  ![image](https://user-images.githubusercontent.com/129961356/230736224-d67316fe-e808-48bd-b35c-af87da5aefa7.png)
36. Once connected, you will get to see a list of repositories under your GitHub account. Select our newly created repository which holds the .NET 6 Web API code. Select the default branch, which is the master.

More importantly, ensure that you have selected the ‘Start the pipeline on source code change’. This makes sure that whenever there is a code change pushed to the master branch of your repository, the pipeline will be re-triggered! Click Next.
37. Next, is where we define the build stage of the pipeline. Here, select the provider as AWS CodeBuild. Create a new AWS CodeBuild project by clicking Create Project.
38. There will be a new popup for creating a new build project. Give it a name.
39. Under the environment configuration, select Ubuntu, Runtime as standard, and Image as standard 6.0 which is the latest one
40.Under the Buildspec configuration, select use a buildspec file
41. Leave the remaining as the default, and create ‘continue to pipeline’. This would create a new CodeBuild project, close the popup and load the newly created project details into the code pipeline setup.
42. As you can see, the project name is loaded. That’s it. Click on Next.
43. Next comes the deploy stage. Here, select the provider as Elastic Beanstalk. There are other options as well like ECS and stuff. let’s select Beanstalk as it’s a far simpler way of deployment. Make sure to select the app and environment that we had already created in the first step. Done, click on next. You will be presented with a Review Page.
44. Create the pipline
45. ![image](https://user-images.githubusercontent.com/129961356/230736720-8498b843-9620-4776-b058-4bbff9200e25.png)
46. Now access the env url.
  you can see 
  ![image](https://user-images.githubusercontent.com/129961356/230736839-e080c3c2-a1c4-41ec-9277-10abd81b2802.png)
  its is showing because i have added the following line of code in program.cs
  app.MapGet("/", () => "Hello World!");
47. Now you can change in this line and commit the code Pipeline will automatically build and deploy the code. 
  
Note:
  All Apis are working correctly on local machine but on aws have some issue to connect with database, database is created and connected on my local machine and also i m using it to create now databse and adding the data using api's. but facing some connection issue with aws. ![image](https://user-images.githubusercontent.com/129961356/230736990-00909194-d9e7-4810-b2c2-6b74bc3faefa.png)
Rest of the api those are not using databse are working fine. 
  ![image](https://user-images.githubusercontent.com/129961356/230737052-5e7894c3-e90c-4a04-b552-d29f97aaef22.png)
Also authorization is also working fine.
  
  
