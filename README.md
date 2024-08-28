HospitalAdmissionApp is a SPA (Single Page Application) it was created using Microsoft Visual Studio 2022 Community Edition and Microsoft SQL Server 2019 Developer Edition.
The main layout of the client is based on the standard application layout of Radzen Blazor Studio application. The application purpose is to admit patients in a
hospital and store their admission history.

Listed below are instructions to build and run this app:

## Instructions

### Required Resources:
1. [SQL Server Developer Edition](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
2. [Visual Studio 2022 Community Edition](https://visualstudio.microsoft.com/downloads/)

### To build and run this app: 
1. Clone the repository `git clone https://github.com/KwnGou/HospitalAdmissionApp.git`
2. Create the DB using the Microsoft SQL Server Management Studio.
    1. Open the Microsoft SQL Server Management Studio.
    2. Drag and drop the file `HospitalBedsDbScript.sql` into the editor.
    3. Execute the entire file.
3. Open the solution HospitalAdmissionApp.sln.
    1. Run the Server project.

P.S.1 The script uses the default locations for the db files (C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\) if you have a difference on
configuration either adapt the script or create the db manually and remove the CREATE DABASE statement from the script.
P.S.2 If you want to configure the clinics / diseases / rooms / beds provided bellow is the data generation script (`DataGen.sql`).

### Screenshots:

Admission page dropdowngrid for selecting patient:

![image](https://github.com/user-attachments/assets/bdf11665-d155-46f6-ab62-663e8555e4fe)

Selecting available bed:

![image](https://github.com/user-attachments/assets/e760dc4f-a5c2-40c0-873d-37d1b3e518b2)

Notification with room information: 

![image](https://github.com/user-attachments/assets/9e88836e-a34e-42a1-a306-4c7421cb0cd7)

Patients grid:

![image](https://github.com/user-attachments/assets/f1292e6b-3a44-4b3f-8b38-d6253509831c)

Patient details page:

1. Patient info:
   
![image](https://github.com/user-attachments/assets/5590b167-f216-46b7-854c-0091b54d0761)

2. Patient admission history:

![image](https://github.com/user-attachments/assets/252eb2be-2dfe-416b-a906-65e398fe15e5)

Patient creation/edit dialogue:

![image](https://github.com/user-attachments/assets/51a54c6a-5af5-4cba-a2a4-353f96afa136)

Selected clinic rooms overview:

![image](https://github.com/user-attachments/assets/f0bf3d48-90f9-400a-b20f-37106e2285ba)

Clinic configuration:

![image](https://github.com/user-attachments/assets/99edd162-b21d-41ba-afa7-9fcebbcab516)
