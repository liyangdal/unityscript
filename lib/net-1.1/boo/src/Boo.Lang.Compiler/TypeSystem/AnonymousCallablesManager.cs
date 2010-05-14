#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System.Collections;

namespace Boo.Lang.Compiler.TypeSystem
{
	using System;
	using Boo.Lang.Compiler.Ast;
	using Module = Boo.Lang.Compiler.Ast.Module;

	internal class AnonymousCallablesManager
	{
		private TypeSystemServices _tss;
		private Hashtable _cache = new Hashtable();
		private BooCodeBuilder _codeBuilder;

		public AnonymousCallablesManager(TypeSystemServices tss)
		{
			_tss = tss;
			_codeBuilder = new BooCodeBuilder(tss);
		}

		protected TypeSystemServices TypeSystemServices
		{
			get { return _tss; }
		}

		protected BooCodeBuilder CodeBuilder
		{
			get { return _codeBuilder; }
		}

		public AnonymousCallableType GetCallableType(IMethod method)
		{
			CallableSignature signature = new CallableSignature(method);
			return GetCallableType(signature);
		}

		public AnonymousCallableType GetCallableType(CallableSignature signature)
		{
			AnonymousCallableType type = GetCachedCallableType(signature);

			if (type == null)
			{
				type = new AnonymousCallableType(TypeSystemServices, signature);
				_cache.Add(signature, type);
			}
			return type;
		}

		private AnonymousCallableType GetCachedCallableType(CallableSignature signature)
		{
			return (AnonymousCallableType) _cache[signature];
		}

		public IType GetConcreteCallableType(Node sourceNode, CallableSignature signature)
		{
			AnonymousCallableType type = GetCallableType(signature);
			return GetConcreteCallableType(sourceNode, type);
		}

		public IType GetConcreteCallableType(Node sourceNode, AnonymousCallableType anonymousType)
		{
			if (null == anonymousType.ConcreteType)
			{
				anonymousType.ConcreteType = CreateConcreteCallableType(sourceNode, anonymousType);
			}
			return anonymousType.ConcreteType;
		}

		private IType CreateConcreteCallableType(Node sourceNode, AnonymousCallableType anonymousType)
		{
			Module module = TypeSystemServices.GetCompilerGeneratedTypesModule();

			string name = ConcreteCallableTypeNameFor(module, sourceNode);
			ClassDefinition cd = TypeSystemServices.CreateCallableDefinition(name);
			cd.Modifiers |= TypeMemberModifiers.Public;
			cd.LexicalInfo = sourceNode.LexicalInfo;

			cd.Members.Add(CreateInvokeMethod(anonymousType));

			Method beginInvoke = CreateBeginInvokeMethod(anonymousType);
			cd.Members.Add(beginInvoke);

			cd.Members.Add(CreateEndInvokeMethod(anonymousType));
			module.Members.Add(cd);

			CreateCallableTypeBeginInvokeExtensions(anonymousType, beginInvoke);

			return (IType)cd.Entity;
		}

		private static string ConcreteCallableTypeNameFor(Module module, Node sourceNode)
		{
			TypeMember enclosing = EnclosingTypeDefinition(sourceNode);
			string prefix = "";
			string postfix = "";
			if(enclosing != null)
			{
				prefix += enclosing.Name;
				enclosing = EnclosingTypeMember(sourceNode);
				if(enclosing != null)
				{
					prefix += "_" + enclosing.Name;
				}
				prefix += "$";
			}
			else if (!sourceNode.LexicalInfo.Equals(LexicalInfo.Empty))
			{
				prefix += System.IO.Path.GetFileNameWithoutExtension(sourceNode.LexicalInfo.FileName) + "$";
			}
			if(!sourceNode.LexicalInfo.Equals(LexicalInfo.Empty))
			{
				postfix = "$" + sourceNode.LexicalInfo.Line + "_" + sourceNode.LexicalInfo.Column + postfix;
			}
			return "__" + prefix + "callable" + module.Members.Count + postfix + "__";
		}

		private static TypeMember EnclosingTypeMember(Node sourceNode)
		{
			TypeMember ancestorMethod = (TypeMember)sourceNode.GetAncestor(NodeType.Method);
			if (null != ancestorMethod) return ancestorMethod;

			TypeMember ancestorProperty = (TypeMember) sourceNode.GetAncestor(NodeType.Property);
			if (null != ancestorProperty) return ancestorProperty;

			return (TypeMember) sourceNode.GetAncestor(NodeType.Field);
		}

		private static TypeMember EnclosingTypeDefinition(Node sourceNode)
		{
			TypeMember ancestorClass = (TypeMember) sourceNode.GetAncestor(NodeType.ClassDefinition);
			if (null != ancestorClass) return ancestorClass;
			TypeMember ancestorInterface = (TypeMember) sourceNode.GetAncestor(NodeType.InterfaceDefinition);
			if (null != ancestorInterface) return ancestorInterface;
			TypeMember ancestorEnum = (TypeMember) sourceNode.GetAncestor(NodeType.EnumDefinition);
			if (null != ancestorEnum) return ancestorEnum;
			return (TypeMember) sourceNode.GetAncestor(NodeType.Module);
		}

		private void CreateCallableTypeBeginInvokeExtensions(AnonymousCallableType anonymousType, Method beginInvoke)
		{
			ClassDefinition extensions = TypeSystemServices.GetCompilerGeneratedExtensionsClass();
			extensions.Members.Add(CreateBeginInvokeCallbackOnlyExtension(anonymousType, beginInvoke));
			extensions.Members.Add(CreateBeginInvokeSimplerExtension(anonymousType, beginInvoke));
		}

		Method CreateBeginInvokeMethod(ICallableType anonymousType)
		{
			Method method = CodeBuilder.CreateRuntimeMethod("BeginInvoke", TypeSystemServices.Map(typeof(IAsyncResult)),
															anonymousType.GetSignature().Parameters, false);

			int delta = method.Parameters.Count;
			method.Parameters.Add(
					CodeBuilder.CreateParameterDeclaration(delta + 1, "callback", TypeSystemServices.Map(typeof(AsyncCallback))));
			method.Parameters.Add(
					CodeBuilder.CreateParameterDeclaration(delta + 1, "asyncState", TypeSystemServices.ObjectType));
			return method;
		}

		Method CreateBeginInvokeExtension(ICallableType anonymousType, Method beginInvoke, out MethodInvocationExpression mie)
		{
			InternalMethod beginInvokeEntity = (InternalMethod)beginInvoke.Entity;

			Method extension = CodeBuilder.CreateMethod("BeginInvoke", TypeSystemServices.Map(typeof(IAsyncResult)),
										TypeMemberModifiers.Public | TypeMemberModifiers.Static);
			extension.Attributes.Add(CodeBuilder.CreateAttribute(Types.BooExtensionAttribute));
			
			ParameterDeclaration self = CodeBuilder.CreateParameterDeclaration(0, "self", beginInvokeEntity.DeclaringType);

			extension.Parameters.Add(self);
			CodeBuilder.DeclareParameters(extension, anonymousType.GetSignature().Parameters, 1);

			mie = CodeBuilder.CreateMethodInvocation(
						CodeBuilder.CreateReference(self),
						beginInvokeEntity);

			ParameterDeclarationCollection parameters = extension.Parameters;
			for (int i = 1; i < parameters.Count; ++i)
			{
				mie.Arguments.Add(CodeBuilder.CreateReference(parameters[i]));
			}
			extension.Body.Add(new ReturnStatement(mie));
			return extension;
		}

		Method CreateBeginInvokeSimplerExtension(ICallableType anonymousType, Method beginInvoke)
		{
			MethodInvocationExpression mie;
			Method overload = CreateBeginInvokeExtension(anonymousType, beginInvoke, out mie);

			mie.Arguments.Add(CodeBuilder.CreateNullLiteral());
			mie.Arguments.Add(CodeBuilder.CreateNullLiteral());

			return overload;
		}

		Method CreateBeginInvokeCallbackOnlyExtension(ICallableType anonymousType, Method beginInvoke)
		{
			MethodInvocationExpression mie;

			Method overload = CreateBeginInvokeExtension(anonymousType, beginInvoke, out mie);
			ParameterDeclaration callback = CodeBuilder.CreateParameterDeclaration(overload.Parameters.Count,
										"callback", TypeSystemServices.Map(typeof(AsyncCallback)));
			overload.Parameters.Add(callback);

			mie.Arguments.Add(CodeBuilder.CreateReference(callback));
			mie.Arguments.Add(CodeBuilder.CreateNullLiteral());

			return overload;
		}

		public Method CreateEndInvokeMethod(ICallableType anonymousType)
		{
			CallableSignature signature = anonymousType.GetSignature();
			Method method = CodeBuilder.CreateRuntimeMethod("EndInvoke", signature.ReturnType);

			int delta = 1;
			foreach (IParameter p in signature.Parameters)
			{
				if (p.IsByRef)
				{
					method.Parameters.Add(
						CodeBuilder.CreateParameterDeclaration(++delta,
								p.Name,
								p.Type,
								true));
				}
			}
			delta = method.Parameters.Count;
			method.Parameters.Add(
				CodeBuilder.CreateParameterDeclaration(delta + 1, "result", TypeSystemServices.Map(typeof(IAsyncResult))));
			return method;
		}

		Method CreateInvokeMethod(AnonymousCallableType anonymousType)
		{
			CallableSignature signature = anonymousType.GetSignature();
			return CodeBuilder.CreateRuntimeMethod("Invoke", signature.ReturnType, signature.Parameters, signature.AcceptVarArgs);
		}
	}
}
