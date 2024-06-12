using SixLabors.ImageSharp.Textures.PixelFormats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace XCOM_Uncooker.Unreal.Shaders
{
    public struct FMaterialUniformExpressionAppendVector : IUnrealSerializable
    {
        #region Serialized data

        public FMaterialUniformExpression A; 

        public FMaterialUniformExpression B; 
        
        public uint NumComponentsA;

        #endregion

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            throw new NotImplementedException();
        }

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(ref A);
            stream.Object(ref B);
            stream.UInt32(ref NumComponentsA);
        }
    }

    public struct FMaterialUniformExpressionClamp : IUnrealSerializable
    {
        #region Serialized data

        public FMaterialUniformExpression Input;

        public FMaterialUniformExpression Min;

        public FMaterialUniformExpression Max;

        #endregion

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            throw new NotImplementedException();
        }

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(ref Input);
            stream.Object(ref Min);
            stream.Object(ref Max);
        }
    }

    public struct FMaterialUniformExpressionConstant : IUnrealSerializable
    {
        #region Serialized data

        public FLinearColor Value;

        public byte ValueType;

        #endregion

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            throw new NotImplementedException();
        }

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(ref Value);
            stream.UInt8(ref ValueType);
        }
    }

    public struct FMaterialUniformExpressionFoldedMath : IUnrealSerializable
    {
        #region Serialized data

        public FMaterialUniformExpression A;

        public FMaterialUniformExpression B;

        public byte Op;

        #endregion

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            throw new NotImplementedException();
        }

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(ref A);
            stream.Object(ref B);
            stream.UInt8(ref Op);
        }
    }

    public struct FMaterialUniformExpressionScalarParameter : IUnrealSerializable
    {
        #region Serialized data

        public FName ParameterName;

        public float DefaultValue;

        #endregion

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            throw new NotImplementedException();
        }

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Name(ref ParameterName);
            stream.Float32(ref DefaultValue);
        }
    }

    public struct FMaterialUniformExpressionSine : IUnrealSerializable
    {
        #region Serialized data

        public FMaterialUniformExpression X;

        public bool bIsCosine; // UBOOL

        #endregion

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            throw new NotImplementedException();
        }

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(ref X);
            stream.BoolAsInt32(ref bIsCosine);
        }
    }

    public struct FMaterialUniformExpressionTexture : IUnrealSerializable
    {
        #region Serialized data

        public int TextureIndex;

        #endregion

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            throw new NotImplementedException();
        }

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(ref TextureIndex);
        }
    }

    public struct FMaterialUniformExpressionTextureParameter : IUnrealSerializable
    {
        #region Serialized data

        public FName ParameterName;

        public int TextureIndex;

        #endregion

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            throw new NotImplementedException();
        }

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Name(ref ParameterName);
            stream.Int32(ref TextureIndex);
        }
    }

    public struct FMaterialUniformExpressionVectorParameter : IUnrealSerializable
    {
        #region Serialized data

        public FName ParameterName;

        public FLinearColor DefaultValue;

        #endregion

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            throw new NotImplementedException();
        }

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Name(ref ParameterName);
            stream.Object(ref DefaultValue);
        }
    }

    public struct FMaterialUniformExpressionXComVectorParameter : IUnrealSerializable
    {
        #region Serialized data

        public byte Value; // no idea what this represents

        #endregion

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            throw new NotImplementedException();
        }

        public void Serialize(IUnrealDataStream stream)
        {
            stream.UInt8(ref Value);
        }
    }

    public struct FMaterialUniformExpression_SingleExpressionInput : IUnrealSerializable
    {
        #region Serialized data

        public FMaterialUniformExpression A;

        #endregion

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            throw new NotImplementedException();
        }

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(ref A);
        }
    }

    public struct FMaterialUniformExpression_DoubleExpressionInput : IUnrealSerializable
    {
        #region Serialized data

        public FMaterialUniformExpression A;

        public FMaterialUniformExpression B;

        #endregion

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            throw new NotImplementedException();
        }

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(ref A);
            stream.Object(ref B);
        }
    }

    public struct FMaterialUniformExpression : IUnrealSerializable
    {
        #region Serialized data

        public FName TypeName; // which type of expression this is

        public IUnrealSerializable Expression; // an object of the corresponding expression type

        #endregion

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            throw new NotImplementedException();
        }

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Name(ref TypeName);

            if (stream.IsRead)
            {
                switch (TypeName.ToString())
                {
                    case "FMaterialUniformExpressionAbs":
                    case "FMaterialUniformExpressionCeil":
                    case "FMaterialUniformExpressionFloor":
                    case "FMaterialUniformExpressionFrac":
                    case "FMaterialUniformExpressionLength":
                    case "FMaterialUniformExpressionPeriodic":
                    case "FMaterialUniformExpressionSquareRoot":
                        var singleExpInput = new FMaterialUniformExpression_SingleExpressionInput();
                        stream.Object(ref singleExpInput);
                        Expression = singleExpInput;
                        break;
                    case "FMaterialUniformExpressionFmod":
                    case "FMaterialUniformExpressionMax":
                    case "FMaterialUniformExpressionMin":
                        var doubleExpInput = new FMaterialUniformExpression_DoubleExpressionInput();
                        stream.Object(ref doubleExpInput);
                        Expression = doubleExpInput;
                        break;
                    case "FMaterialUniformExpressionAppendVector":
                        var appendVector = new FMaterialUniformExpressionAppendVector();
                        stream.Object(ref appendVector);
                        Expression = appendVector;
                        break;
                    case "FMaterialUniformExpressionClamp":
                        var clamp = new FMaterialUniformExpressionClamp();
                        stream.Object(ref clamp);
                        Expression = clamp;
                        break;
                    case "FMaterialUniformExpressionConstant":
                        var constant = new FMaterialUniformExpressionConstant();
                        stream.Object(ref constant);
                        Expression = constant;
                        break;
                    case "FMaterialUniformExpressionFoldedMath":
                        var foldedMath = new FMaterialUniformExpressionFoldedMath();
                        stream.Object(ref foldedMath);
                        Expression = foldedMath;
                        break;
                    case "FMaterialUniformExpressionScalarParameter":
                        var scalarParameter = new FMaterialUniformExpressionScalarParameter();
                        stream.Object(ref scalarParameter);
                        Expression = scalarParameter;
                        break;
                    case "FMaterialUniformExpressionSine":
                        var sine = new FMaterialUniformExpressionSine();
                        stream.Object(ref sine);
                        Expression = sine;
                        break;
                    case "FMaterialUniformExpressionTexture":
                    case "FMaterialUniformExpressionFlipBookTextureParameter":
                        var texture = new FMaterialUniformExpressionTexture();
                        stream.Object(ref texture);
                        Expression = texture;
                        break;
                    case "FMaterialUniformExpressionTextureParameter":
                        var textureParameter = new FMaterialUniformExpressionTextureParameter();
                        stream.Object(ref textureParameter);
                        Expression = textureParameter;
                        break;
                    case "FMaterialUniformExpressionVectorParameter":
                        var vectorParameter = new FMaterialUniformExpressionVectorParameter();
                        stream.Object(ref vectorParameter);
                        Expression = vectorParameter;
                        break;
                    case "FMaterialUniformExpressionXComVectorParameter":
                        var xcomVectorParameter = new FMaterialUniformExpressionXComVectorParameter();
                        stream.Object(ref xcomVectorParameter);
                        Expression = xcomVectorParameter;
                        break;
                    // Deliberately blank, these have no inputs
                    case "FMaterialUniformExpressionRealTime":
                    case "FMaterialUniformExpressionTime":
                        break;
                    default:
                        Debugger.Break();
                        break;
                }
            }
            else
            {
                // Expression may be null if no extra data is needed beyond its type
                if (Expression != null)
                {
                    // Can't use stream.Object because we don't have a concrete type to match the new() constraint
                    Expression.Serialize(stream);
                }
            }
        }
    }


    public struct FShaderFrequencyUniformExpressions : IUnrealSerializable
    {
        #region Serialized data

        public FMaterialUniformExpression[] UniformVectorExpressions;
        public FMaterialUniformExpression[] UniformScalarExpressions;
        public FMaterialUniformExpression[] Uniform2DTextureExpressions;

        #endregion
     
        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            throw new NotImplementedException();
        }

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Array(ref UniformVectorExpressions);
            stream.Array(ref UniformScalarExpressions);
            stream.Array(ref Uniform2DTextureExpressions);
        }
    }

    public struct FUniformExpressionSet : IUnrealSerializable
    {
        #region Serialized data

        public FShaderFrequencyUniformExpressions PixelExpressions;
        public FMaterialUniformExpression[] UniformCubeTextureExpressions;
        public FShaderFrequencyUniformExpressions VertexExpressions;
        public FShaderFrequencyUniformExpressions HullExpressions;
        public FShaderFrequencyUniformExpressions DomainExpressions;

        #endregion

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            throw new NotImplementedException();
        }

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(ref PixelExpressions);
            stream.Array(ref UniformCubeTextureExpressions);
            stream.Object(ref VertexExpressions);
            stream.Object(ref HullExpressions);
            stream.Object(ref DomainExpressions);
        }
    }
}
