package Output.Java;

import Output.Java.Example.*;

public class FromCSharp extends Base implements IInterface
{
    public int number = 0;
    public float weight;
    private String stringPropertyBackingField;
    private int intPropertyBackingField;
    
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
    
    private static int add(int[] numbers,int count)
    {
        var sum = 0;
        for (var i = 0; i < count; i++)
        {
            sum += numbers[i];
        }
        return sum;
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

