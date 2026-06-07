Task
Create an application for password reset using brute force attack and multi-threaded method.
Application must have a graphical interface with:
password creation
start/stop brute-force attack
progress display
elapsed time display
result output
3.       Each major functionality must be implemented in a separate class stored in a separate file.

4.       The program must have the following options:

a.       Password must be hashed using SHA256 with a constant static salt defined in the application ;

b.      Password length must be randomly generated between [4–6) characters

c.       Brute force attack must generate all possible combinations of characters starting from length 1 up to a maximum length of 6. The brute force algorithm must NOT know the password length in advance and must begin searching from length 1.)

d.      Must use multi-threading (Thread or Task-based)

e.         Must use maximum of (CPU cores - 1)

f.        GUI must include:

·         start/stop button

·         progress indicator

·         elapsed time display

·         found password output

   5.    5. The application must demonstrate parallel execution (multiple threads working simultaneously), not sequential brute force.

6.      After finding the password, all running threads must be stopped immediately

7.      Brute force generator and validator must be implemented separately and independently.

8.      Application must log performance difference between:

·         single-thread brute force

·         multi-thread brute force




Commits
1. Set a Avalonia App
   > dotnet new Avalonia.Templates
   > dotnet new avalonia.app -o password
   > cd password
   > dotnet run

2. Added MainWindow GUI
   > Added Button like Generate buttons, Start Attack, Stop Attack
   > Added Radio selection method
   > Added Progress GUI
   > Added Elapsed Time GUI
   > Added Box for Result

3. Added PasswordHasher and Changes some MainWindow
   > Pushed the PasswordHasher Model
   > It handles password hashing using SHA256 with a constant static salt.
   > It Generates a SHA256 hash of the given password with the static salt.
   > It Performs constant-time string comparison to prevent timing attacks.
  For MainWindow
   > Differentiate the code section wise.
   > Made more Reactive GUI

4. Added PasswordGenerator
   > Pushed PasswordGenerator model
   > It Generates random passwords for testing the brute-force algorithm.
   > It set character for password generation (lowercase and digits for simplicity)
   > Minimum password length - 2, (inclusive) and Maximum length - 4 (exclusive)

5. Added the BruteForce and the Password Validator
   > Pushed the BruteForce and password validator
   > It generates all possible character combinations for brute-force attacks.
   > It modifies the array in place and yields complete combinations.
   > Gets the character set used for generation.
   > Gets the minimum length - 1 of combinations.
   > Gets the maximum length - 4 of combinations.
   > Calculates the total number of possible combinations.
   Password Validator
   > It validates passwords against a target hash.
   > This is Stage 2 of the development - Core validation functionality.
   > It sets the target hash that passwords will be validated against.
   > It checks if the given password is correct by comparing its hash with the target hash.
   > This method is thread-safe as it only reads from _targetHash and uses
   > It verify password against the target hash
   > It gets the current target hash.
   > It resets the validator state.

6. Added MultiThread and SingleThread
   MultiThreading
   > Pushed the Multi-ThreadingBruteForce.cs
   > It performs brute-force password cracking using multiple threads.
   > It uses (CPU cores - 1) threads for parallel processing
   > It supports cancellation tokens for graceful shutdown
   > It thread-safe implementation using thread-safe collections
   > It stops all threads immediately when password is found
   > It Acts bit slow
   SingleThreading
   > It performs a brute-force password cracking attack using a single thread.
   > This serves as the baseline for performance comparison with multi-threaded version.
   > It cracks the password by trying all combinations sequentially.
   > It generates and tests combinations sequentially, starting from length 1.

7. Added Crack Result
   > Pushed CrackResult.cs
   > It represents the result of a brute-force password cracking attempt.
   > It gives result for password that was found, or empty string if not found.\
   > It gives total time elapsed during the cracking attempt.
   > It gives total number of password attempts made.
   > It provides Number of threads used in the cracking attempt.
   > It provides attempts per second (calculated metric).

8. Added Performance Logger
   > Pushed Performance logger
   > It logs and compares performance metrics between single and multi-threaded brute force.
   > It gives the complete performance report.
   > It clears all logged results.
   > It also gives a summary of both results side-by-side.

9. Added ViewModel Folder and file
    > Pushed ViewModel Folder and MainWindowViewModel.cs
    > This is ViewModel for the main window using MVVM pattern.
    > It Update progress based on percentage.
    > It uses DispatcherTimer which runs on UI thread.
    Changes MainWindow.axaml
    > At first Selecting SingleThread and MultiThread was not working properly
    > Changed the properties of Radio GUI
    PerformanceLogger
    > Added using password.ViewModel

10. Made Some changes in MainWindow.axaml.cs as it has some running problem
    > Added ResultBox
    > Added radio button handlers class like SingleThreadRadio_Click and MultiThreadRadio_Click

11. Changes Main Window Progess
    > After all the file compilation the program succesfully run
    > I commit the full made file once again and progress of background like assembly and cache for Avalonia got commit

12. Added ReadMe
    > ReadMe file with all the commit and chages explained

To run
> Debug file containing App
> or Opening Folder in VS code
   > Terminal
   > Dotnet run
