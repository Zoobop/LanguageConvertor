# Language Convertor

* File Translation
* File Building

# File Building

### File Configuration
![File Configuration](https://github.com/Zoobop/LanguageConvertor/blob/master/gitImages/building_Git.Image_01.png "File Configuration")

To setup for **File Building**, you must first construct a ```FileBuilderConfig``` object for the language of your choosing. (Ex: ```CppBuilderConfig```)
Then, you must construct a ```FilePackBuilder``` object and pass in your config variable. The rest of the configuration is done for you.

### Building A File
![Building A File](https://github.com/Zoobop/LanguageConvertor/blob/master/gitImages/building_Git.Image_02.png "Building A File")

To add an import, simply call the ```CreateImport()``` method from your ```FileBuilderConfig``` object and pass in its arguments.
Its arguments include:
* `name`: import name
* `isBuiltIn`: is the import built into the language

Containers, in this context, pertain to namespaces and packages. To ensure the proper encapsulation of components (classes, methods, fields), there are two methods to start and end a container.
To add a namespace/package, simply call the ```StartContainer()``` method from your ```FileBuilderConfig``` object and pass in its arguments. After you are done adding things to the container, call the ```EndContainer()``` method to end container.
Its arguments include:
* `name`: container name
* `isFileScoped`: is the container file-scoped

Since classes also encapsulate components, there are also the two start and end methods.
To add a class, simply call the ```StartClass()``` method from your ```FileBuilderConfig``` object and pass in its arguments. After you are done adding things to the class, call the ```EndClass()``` method to end class.
Its arguments include:
* `name`: class name
* `accessModifier`: access modifier of the class
* `specialModifier`: special modifier (static, abstract, etc...)
* `parentClass`: inherited parent class
* `interfaces`: inherited interfaces

To add a method/function, simply call the ```CreateMethod()``` method from your ```FileBuilderConfig``` object and pass in its arguments.
Its arguments include:
* `name`: method name
* `type`: return type
* `accessModifier`: access modifier of the method
* `specialModifier`: special modifier (static, abstract, etc...)
* `body`: body of the method (code block)
* `parameters`: parameters of the method

To add a field to your class, simply call the ```CreateField()``` method from your ```FileBuilderConfig``` object and pass in its arguments.
Its arguments include:
* `name`: field name
* `type`: field type
* `value`: value at declaration
* `accessModifier`: access modifier of the field
* `specialModifier`: special modifier (static, const, etc...)

### Output To File
![Output To File](https://github.com/Zoobop/LanguageConvertor/blob/master/gitImages/building_Git.Image_03.png "Output")

To recieve the result of the build, construct a ```Convertor``` object, and pass the data in your ```FilePackBuilder``` object calling the ```GetData()``` method.
This converts your components into an `IEnumerable<string>` behind the scenes, which holds the lines of the formatted language.
You can get that formatted data directly by calling the ```GetData()``` method from your ```Convertor``` object for a single `string` or ```GetDataAsLines()``` as an `IEnumerable<string>`, or if you want to send that data directly to a file, call the ```ToFile()``` method, passing in the output path for where you want your file to go.

The above figure shows both cases.

### Results
![Output](https://github.com/Zoobop/LanguageConvertor/blob/master/gitImages/building_Git.Image_04.png "Output")


