namespace LanguageConvertor.Core;

public delegate string Format();
public delegate string Format<in T0>(T0 arg);
public delegate string Format<in T0, in T1>(T0 arg0, T1 arg1);
public delegate string Format<in T0, in T1, in T2>(T0 arg0, T1 arg1, T2 arg2);

public abstract class FileBuilderConfig
{
    /* ----------------------- LANGUAGE ----------------------- */
    internal ConvertibleLanguage Language { get; init; }
    
    /* -------------------- GENERAL FORMAT -------------------- */
    internal Format DefaultValueFormat { get; init; }
    internal Format<string> NewStackAllocationFormat { get; init; }
    internal Format<string> NewHeapAllocationFormat { get; init; }

    /* -------------- CONSTRUCTOR SPECIFIC FORMAT ------------- */
    internal Format<string> ConstructorNameFormat { get; init; }
    internal Format<string, string> MemberInitializationFormat { get; init; }

    /* -------------------- METHOD FORMAT --------------------- */
    internal Format<string> ParameterNameFormat { get; init; }
}