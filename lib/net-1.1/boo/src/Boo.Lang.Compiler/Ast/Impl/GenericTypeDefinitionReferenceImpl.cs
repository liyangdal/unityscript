#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
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

//
// DO NOT EDIT THIS FILE!
//
// This file was generated automatically by astgen.boo.
//
namespace Boo.Lang.Compiler.Ast
{	
	using System.Collections;
	using System.Runtime.Serialization;
	
	[System.Serializable]
	public  class GenericTypeDefinitionReferenceImpl : SimpleTypeReference
	{
		public GenericTypeDefinitionReferenceImpl()
		{
		}

		public GenericTypeDefinitionReferenceImpl(LexicalInfo lexicalInfo) : base(lexicalInfo)
		{
		}
		protected int _genericPlaceholders;


		new public GenericTypeDefinitionReference CloneNode()
		{
			return Clone() as GenericTypeDefinitionReference;
		}

		override public NodeType NodeType
		{
			get
			{
				return NodeType.GenericTypeDefinitionReference;
			}
		}

		override public void Accept(IAstVisitor visitor)
		{
			visitor.OnGenericTypeDefinitionReference((GenericTypeDefinitionReference)this);
		}

		override public bool Matches(Node node)
		{	
			GenericTypeDefinitionReference other = node as GenericTypeDefinitionReference;
			if (null == other) return false;
			if (_name != other._name) return NoMatch("GenericTypeDefinitionReference._name");
			if (_genericPlaceholders != other._genericPlaceholders) return NoMatch("GenericTypeDefinitionReference._genericPlaceholders");
			return true;
		}

		override public bool Replace(Node existing, Node newNode)
		{
			if (base.Replace(existing, newNode))
			{
				return true;
			}
			return false;
		}

		override public object Clone()
		{
			GenericTypeDefinitionReference clone = (GenericTypeDefinitionReference)FormatterServices.GetUninitializedObject(typeof(GenericTypeDefinitionReference));
			clone._lexicalInfo = _lexicalInfo;
			clone._endSourceLocation = _endSourceLocation;
			clone._documentation = _documentation;
			if (_annotations != null) clone._annotations = (Hashtable)_annotations.Clone();
		
			clone._name = _name;
			clone._genericPlaceholders = _genericPlaceholders;
			return clone;
		}

		override internal void ClearTypeSystemBindings()
		{
			_annotations = null;

		}
	

		[System.Xml.Serialization.XmlElement]
		public int GenericPlaceholders
		{
			get
			{

				return _genericPlaceholders;
			}

			set
			{
				_genericPlaceholders = value;
			}

		}
		

	}
}
