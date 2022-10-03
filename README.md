# Language Convertor

## File Configuration
![File Configuration](https://github.com/Zoobop/LanguageConvertor/blob/master/gitImages/Language_Convertor_Git.Image_01.png "File Configuration")

When the program starts, the application interface appears and is used similarly to a typical command line. The short list of comaands are displayed below.

## Database Commands
![Database Commands](https://github.com/Zoobop/LanguageConvertor/blob/master/gitImages/accountGen_i2.png "Database Commands")

By inputing the '**help**' command, as described at the top of the screen, it displays the list of commands used to interact with the local database.

Codewise, the data structures used to store information such as the 'commands' and the 'database registry' were ```mtk::Map<TKey, TVal>``` and ```mtk::List<T>``` respectively. There were also a few string utility methods used from to help with parsing user input.

## Adding New User (Manual)
![Adding New User](https://github.com/Zoobop/LanguageConvertor/blob/master/gitImages/accountGen_i3.png "Adding New User")

The user input requirements for adding a new user are not too strict, as this application's purpose was mainly to demonstrate a small, simple project using the MicroToolKit.

To save new user to the database, you must also '**commit**' to see a persistent change.

## Generating New User (Automatic)
![Generating New User](https://github.com/Zoobop/LanguageConvertor/blob/master/gitImages/accountGen_i4.png "Generating New User")

The '**generate**' is the main function of this program. The implementation of this function used ```mtk::List<T>``` multiple times to store random names and symbols. The ```mtk::Random``` utility class was also used to randomize the values of the users' email, username, and password. The command has one parameter 'amount' that is required to specify the number of new accounts to generate.

Remember: to see a persistent change in the local database, you must use the '**commit**' command after successful generation.
