using LanguageConvertor.Components;

namespace LanguageConvertor.Core;

public sealed class FilePackBuilder
{
    private readonly FilePack _filePack;
    private readonly Stack<IComponent> _scopeStack;
    private readonly FileBuilderConfig _config;
    
    public FilePackBuilder(in FileBuilderConfig config)
    {
        _filePack = new FilePack(config.Language);
        _scopeStack = new Stack<IComponent>();
        _config = config;
    }

    public FilePack GetData() => _filePack;

    #region Building

    public void CreateImport(string import, bool isBuiltIn)
    {
        _filePack.AddImport(new ImportComponent(import, isBuiltIn));
    }
    
    public void StartContainer(string name, bool isFileScoped)
    {
        var containerComponent = new ContainerComponent(name, isFileScoped);
        _filePack.AddContainer(containerComponent);
        
        Feed(containerComponent);
    }

    public void EndContainer()
    {
        _scopeStack.Pop();
    }
    
    public void StartClass(string name, string accessModifier = "", string specialModifier = "", string parentClass = "",
        params string[] interfaces)
    {
        var classComponent = new ClassComponent(accessModifier, specialModifier, name, parentClass, interfaces);
        _filePack.AddClass(classComponent);
        
        Feed(classComponent);
    }

    public void EndClass()
    {
        _scopeStack.Pop();
    }

    public void CreateConstructor(string accessModifier = "public", string specialModifier = "", params ParameterPack[] parameters)
    {
        var classComponent = (ClassComponent)_scopeStack.Peek();
        var constructorName = _config.ConstructorNameFormat(classComponent.Name);
        CreateMethod(constructorName, string.Empty, accessModifier, specialModifier, parameters);
    }
    
    public void CreateInitializerConstructor(string accessModifier = "public", string specialModifier = "")
    {
        var classComponent = (ClassComponent) _scopeStack.Peek();
        var fields = classComponent.Fields;

        var parameters = new ParameterPack[fields.Count];
        for (var i = 0; i < fields.Count; i++)
        {
            var field = fields[i];
            var parameterName = _config.ParameterNameFormat(field.Name);
            parameters[i] = new ParameterPack(parameterName, field.Type);
        }

        var body = new List<string>();
        for (var i = 0; i < fields.Count; i++)
        {
            var field = fields[i];
            var defaultFormat = _config.MemberInitializationFormat(field.Name, parameters[i].Name);
            body.Add(defaultFormat);
        }

        var constructorName = _config.ConstructorNameFormat(classComponent.Name);
        CreateMethod(constructorName, string.Empty, accessModifier, specialModifier, body, parameters);
    }
    
    public void CreateDefaultConstructor(string accessModifier = "public", string specialModifier = "")
    {
        var classComponent = (ClassComponent) _scopeStack.Peek();
        var fields = classComponent.Fields;

        var body = new List<string>();
        if (fields != null)
        {
            for (var i = 0; i < fields.Count; i++)
            {
                var field = fields[i];
                var defaultFieldValue = string.Empty;

                if (field.HasValue)
                {
                    defaultFieldValue = field.Value;
                }
                else
                {
                    defaultFieldValue = (field.IsPointer) ? _config.NewHeapAllocationFormat(field.Type) : _config.NewStackAllocationFormat(field.Type);
                    defaultFieldValue = $"{defaultFieldValue}{_config.DefaultValueFormat()}";
                }

                var assignmentFormat = _config.MemberInitializationFormat(field.Name, defaultFieldValue);
                body.Add(assignmentFormat);
            }
        }

        var constructorName = _config.ConstructorNameFormat(classComponent.Name);
        CreateMethod(constructorName, string.Empty, accessModifier, specialModifier, body);
    }
    
    public void CreateMethod(string name, string type, string accessModifier = "", string specialModifier = "", IEnumerable<string>? body = null, params ParameterPack[] parameters)
    {
        var method = new MethodComponent(accessModifier, specialModifier, type, name);

        if (body != null)
        {
            method.AddToBody(body);
        }

        if (parameters != null)
        {
            method.AddParameters(parameters);
        }

        _filePack.AddMethod(method);
        
        Feed(method);
        _scopeStack.Pop();
    }

    public void CreateMethod(string name, string type, string accessModifier = "", string specialModifier = "", string singleLineBody = "", params ParameterPack[] parameters)
    {
        var method = new MethodComponent(accessModifier, specialModifier, type, name);

        if (!string.IsNullOrEmpty(singleLineBody))
        {
            method.AddToBody(singleLineBody);
        }

        if (parameters != null)
        {
            method.AddParameters(parameters);
        }

        _filePack.AddMethod(method);

        Feed(method);
        _scopeStack.Pop();
    }

    public void CreateMethod(string name, string type, string accessModifier = "", string specialModifier = "", string singleLineBody = "", ParameterPack? parameter = null)
    {
        var method = new MethodComponent(accessModifier, specialModifier, type, name);

        if (!string.IsNullOrEmpty(singleLineBody))
        {
            method.AddToBody(singleLineBody);
        }

        if (parameter != null)
        {
            method.AddParameter(parameter);
        }

        _filePack.AddMethod(method);

        Feed(method);
        _scopeStack.Pop();
    }

    public void CreateMethod(string name, string type, string accessModifier = "", string specialModifier = "", params ParameterPack[] parameters)
    {
        var method = new MethodComponent(accessModifier, specialModifier, type, name);

        if (parameters != null)
        {
            method.AddParameters(parameters);
        }

        _filePack.AddMethod(method);

        Feed(method);
        _scopeStack.Pop();
    }

    public void CreateField(string name, string type, string value = "", string accessModifier = "", string specialModifier = "")
    {
        var field = new FieldComponent(accessModifier, specialModifier, type, name, value);
        _filePack.AddField(field);
        
        Feed(field);
    }
    
    #endregion
    
    #region Helper

    private void Feed(in IComponent component)
    {
        var currentScope = _scopeStack.TryPeek(out var scope) ? scope : null;
        currentScope?.AddComponent(component);
        if (component.IsScope())
        {
            _scopeStack.Push(component);
        }
    }

    #endregion
}