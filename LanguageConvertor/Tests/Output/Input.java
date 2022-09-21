package Output;

import Abstraction.*;

public class FromCSharp
{
    public int number = 0;
    public float weight;
    private String stringPropertyBackingField;
    private int intPropertyBackingField;
    private IAnimal animalBackingField;
    
    public void Method()
    {
        Console.WriteLine("Hello World");
    }
    
    public static void FindAnimal(IAnimal animal)
    {
        
    }
    
    public String getStringProperty()
    {
        return stringPropertyBackingField;
    }
    
    public void setStringProperty(String value)
    {
        stringPropertyBackingField = value;
    }
    
    public int getIntProperty()
    {
        return intPropertyBackingField;
    }
    
    public IAnimal getAnimal()
    {
        return animalBackingField;
    }
    
    private static int Add(int[] numbers,int count)
    {
        var sum = 0;
        for (var i = 0; i < count; i++)
        {
            sum += numbers[i];
        }
        return sum;
    }
    
    void Explode(boolean sure)
    {
        
    }
    
    private void setAnimal(IAnimal value)
    {
        animalBackingField = value;
    }
    
    protected static void Func(String obj)
    {
        
    }
    
}

