using System;
using LuaInterface;
using System.Reflection;

namespace Cother
{
    /// <summary>
    /// Allows this method to be called from Lua scripts.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class LuaScriptFunction : Attribute
    {
        /// <summary>
        /// Lua scripts may call this method via this name.
        /// </summary>
        public String InLuaFunctionName { get; private set; }
        /// <summary>
        /// This attribute allows this method to be called from Lua scripts.
        /// </summary>
        /// <param name="inLuaFunctionName">Lua scripts will call this method via this name.</param>
        public LuaScriptFunction(string inLuaFunctionName)
        {
            InLuaFunctionName = inLuaFunctionName;
        }
    }
    /// <summary>
    /// An interpreter of Lua scripts. Has a self contained virtual machine.
    /// </summary>
    public class LuaWrapper
    {
        /// <summary>
        /// Holds the last exception generated from Lua.
        /// </summary>
        public Exception LastError = null;
        /// <summary>
        /// Holds the inner Lua virtual machine.
        /// </summary>
        private Lua luaVM;

        /// <summary>
        /// Registers all methods of the container that are marked with the appropriate attribute (LuaScriptFunction).
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public void RegisterAttributeBasedFunctions(object container)
        {
            if (container == null || luaVM == null)
            {
                throw new ArgumentNullException("container", "Registrating functions failed.");
            }
            Type containerType = container.GetType();
            foreach(MethodInfo mInfo in containerType.GetMethods())
            {
                foreach(Attribute attr in Attribute.GetCustomAttributes(mInfo))
                {
                    if (attr.GetType() == typeof(LuaScriptFunction))
                    {
                        LuaScriptFunction attribute = (LuaScriptFunction)attr;
                        string luaName = attribute.InLuaFunctionName;
                        luaVM.RegisterFunction(luaName, container, mInfo);
                    }
                }
            }
        }
        /// <summary>
        /// Returns the value of the specified Lua global variable. Throws an exception if the variable does not exist.
        /// </summary>
        public object GetGlobal(string name)
        {
            object o = luaVM[name];
            if (o == null)
            {
                throw new Exception("Lua global variable named '" + name + "' does not exist.");
            }
            else return o;
        }
        /// <summary>
        /// Returns the value of a global variable within the Lua Virtual Machine. May throw an exception or perhaps return null.
        /// </summary>
        public object GetGlobalUnsafe(string name)
        {
            return luaVM[name];
        }
        /// <summary>
        /// Creates or overwrites a global variable within the LuaVM.
        /// </summary>
        /// <param name="name">Name of the global variable.</param>
        /// <param name="value">Value of the global variable.</param>
        public void SetGlobal(string name, object value)
        {
            luaVM[name] = value;
        }
        /// <summary>
        /// Registers a C# method so that it may be used from within Lua.
        /// </summary>
        /// <param name="functionName">The name this function will have from within Lua.</param>
        /// <param name="csharpFunctionOwner">The object that has this method.</param>
        /// <param name="csharpFunction">Reference to the function.</param>
        public void RegisterFunction(string functionName, object csharpFunctionOwner, MethodBase csharpFunction)
        {
            luaVM.RegisterFunction(functionName, csharpFunctionOwner, csharpFunction);
        }
        /// <summary>
        /// Calls a function defined in the Lua VM.
        /// </summary>
        /// <param name="functionName">The Lua function name.</param>
        /// <param name="parameters">Parameters to be passed to the Lua function.</param>
        public object[] CallFunction(string functionName, params object[] parameters)
        {
            LuaFunction f = luaVM.GetFunction(functionName);
            object[] returnValues = f.Call(parameters);
            return returnValues;
        }
        /// <summary>
        /// Returns true if the LuaVM contains a global function of this name.
        /// </summary>
        public bool FunctionExists(string functionName)
        {
            LuaFunction f = luaVM.GetFunction(functionName);
            return f != null;
        }
        
        /// <summary>
        /// Attempts to retrive the value of a global variable from Lua.
        /// If such a variable does not exist, this returns null.
        /// </summary>
        /// <param name="name">Name of the global Lua variable.</param>
        /// <param name="exception">If an exception was generated, it is suppressed and given here (and to LastError).</param>
        /// <returns></returns>
        public object TryGetGlobal(string name, out Exception exception)
        {
            try
            {
                exception = null;
                return luaVM[name];
            }
            catch (Exception ex)
            {
                exception = ex;
                LastError = ex;
                return null;
            }
        }
        /// <summary>
        /// Executes the Lua source code passed as parameter.
        /// </summary>
        /// <param name="luaScriptText">The Lua source code to execute.</param>
        public bool Execute(string luaScriptText)
        {
            try
            {
                luaVM.DoString(luaScriptText);
                return true;
            }
            catch (Exception e)
            {
                LastError = e;
                return false;
            }
        }
        /// <summary>
        /// Executes the Lua script found in the specified file.
        /// </summary>
        /// <param name="luaFileName">Lua script to execute.</param>
        public bool ExecuteFile(string luaFileName)
        {
            try
            {
                luaVM.DoFile(luaFileName);
                return true;
            }
            catch (Exception e)
            {
                LastError = e;
                return false;
            }
        }
        /// <summary>
        /// Resets the Lua virtual machine. This unregisters all functions.
        /// </summary>
        public void Reset()
        {
            luaVM = new Lua();
        }
        /// <summary>
        /// Creates a new wrapper around a new Lua virtual machine. Then you can use this wrapper to call Lua scripts.
        /// </summary>
        public LuaWrapper()
        {
            Reset();
        }
    }
}
