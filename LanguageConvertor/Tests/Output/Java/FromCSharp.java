package Output.Java;

import Output.Java.Example.*;

public class FromCSharp extends Base implements IInterface
{
    public int number = 0;
    public float weight;
    private String stringPropertyBackingField;
    private int intPropertyBackingField;
    
    public FromCSharp()
    {
        StringProperty = "NULL";
        IntProperty = 0;
    }
    
    public FromCSharp(String str, int integer)
    {
        StringProperty = str;
        IntProperty = integer;
    }
    
    public void method()
    {
    }
    
    public void func3(Object obj)
    {
    }
    
    public String getStringProperty()
    {
        return stringPropertyBackingField;
    }
    
    public int getIntProperty()
    {
        return intPropertyBackingField;
    }
    
    private static int add(int[] numbers, int count)
    {
        return 0;
    }
    
    void explode(boolean sure)
    {
    }
    
    @Override
    protected void func1(int obj)
    {
    }
    
    @Override
    protected void func2(String obj)
    {
    }
    
    protected void setStringProperty(String value)
    {
        stringPropertyBackingField = value;
    }
    
}

