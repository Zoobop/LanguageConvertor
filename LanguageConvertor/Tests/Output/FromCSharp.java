package Output;

import Abstraction.*;

public class FromCSharp extends Base implements IInterface
{
    public int number = 0;
    public float weight;
    private String stringPropertyBackingField;
    private int intPropertyBackingField;
    private IAnimal animalBackingField;
    
    public void method()
    {
        
    }
    
    public static void findAnimal(IAnimal animal)
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
    
    private static int add(int[] numbers,int count)
    {
        var sum = 0;
        for (var i = 0; i < count; i++)
        {
            sum += numbers[i];
        }
        return sum;
    }
    
    void explode(bool sure)
    {
        
    }
    
    private void setAnimal(IAnimal value)
    {
        animalBackingField = value;
    }
    
    protected static void func(String obj)
    {
        
    }
    
}

