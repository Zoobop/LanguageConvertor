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

The structure of building a file should follow the standard language format:
* Imports
* Container/Namespace
* Class
* Function (if applicable)

To add an import, simply call the ```CreateImport``` method from your ```FileBuilderConfig``` object and pass in its arguments.
Its arguments include:
* name - import name
* isBuiltIn - is the import built into the language

Containers, in this context, pertain to namespaces and packages. To ensure the proper encapsulation of components (classes, methods, fields), there are two methods to start and end a container.
To add a namespace/package, simply call the ```StartContainer``` method from your ```FileBuilderConfig``` object and pass in its arguments. At the end of the container, call the ```EndContainer``` method to end container.
Its arguments include:
* name - container name
* isFileScoped - is the container file-scoped

### Output To File
![Output To File](https://github.com/Zoobop/LanguageConvertor/blob/master/gitImages/building_Git.Image_03.png "Output")



### Results
![Output](https://github.com/Zoobop/LanguageConvertor/blob/master/gitImages/building_Git.Image_04.png "Output")


