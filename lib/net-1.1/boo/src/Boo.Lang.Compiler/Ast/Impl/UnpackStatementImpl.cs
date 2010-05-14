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
	public  class UnpackStatementImpl : Statement
	{
		public UnpackStatementImpl()
		{
		}

		public UnpackStatementImpl(LexicalInfo lexicalInfo) : base(lexicalInfo)
		{
		}
		protected DeclarationCollection _declarations;

		protected Expression _expression;


		new public UnpackStatement CloneNode()
		{
			return Clone() as UnpackStatement;
		}

		override public NodeType NodeType
		{
			get
			{
				return NodeType.UnpackStatement;
			}
		}

		override public void Accept(IAstVisitor visitor)
		{
			visitor.OnUnpackStatement((UnpackStatement)this);
		}

		override public bool Matches(Node node)
		{	
			UnpackStatement other = node as UnpackStatement;
			if (null == other) return false;
			if (!Node.Matches(_modifier, other._modifier)) return NoMatch("UnpackStatement._modifier");
			if (!Node.AllMatch(_declarations, other._declarations)) return NoMatch("UnpackStatement._declarations");
			if (!Node.Matches(_expression, other._expression)) return NoMatch("UnpackStatement._expression");
			return true;
		}

		override public bool Replace(Node existing, Node newNode)
		{
			if (base.Replace(existing, newNode))
			{
				return true;
			}
			if (_modifier == existing)
			{
				this.Modifier = (StatementModifier)newNode;
				return true;
			}
			if (_declarations != null)
			{
				Declaration item = existing as Declaration;
				if (null != item)
				{
					Declaration newItem = (Declaration)newNode;
					if (_declarations.Replace(item, newItem))
					{
						return true;
					}
				}
			}
			if (_expression == existing)
			{
				this.Expression = (Expression)newNode;
				return true;
			}
			return false;
		}

		override public object Clone()
		{
			UnpackStatement clone = (UnpackStatement)FormatterServices.GetUninitializedObject(typeof(UnpackStatement));
			clone._lexicalInfo = _lexicalInfo;
			clone._endSourceLocation = _endSourceLocation;
			clone._documentation = _documentation;
			if (_annotations != null) clone._annotations = (Hashtable)_annotations.Clone();
		
			if (null != _modifier)
			{
				clone._modifier = _modifier.Clone() as StatementModifier;
				clone._modifier.InitializeParent(clone);
			}
			if (null != _declarations)
			{
				clone._declarations = _declarations.Clone() as DeclarationCollection;
				clone._declarations.InitializeParent(clone);
			}
			if (null != _expression)
			{
				clone._expression = _expression.Clone() as Expression;
				clone._expression.InitializeParent(clone);
			}
			return clone;
		}

		override internal void ClearTypeSystemBindings()
		{
			_annotations = null;
			if (null != _modifier)
			{
				_modifier.ClearTypeSystemBindings();
			}
			if (null != _declarations)
			{
				_declarations.ClearTypeSystemBindings();
			}
			if (null != _expression)
			{
				_expression.ClearTypeSystemBindings();
			}

		}
	

		[System.Xml.Serialization.XmlElement]
		public DeclarationCollection Declarations
		{
			get
			{

			if (_declarations == null) _declarations = new DeclarationCollection(this);

				return _declarations;
			}

			set
			{
				if (_declarations != value)
				{
					_declarations = value;
					if (null != _declarations)
					{
						_declarations.InitializeParent(this);
					}
				}
			}

		}
		

		[System.Xml.Serialization.XmlElement]
		public Expression Expression
		{
			get
			{

				return _expression;
			}

			set
			{
				if (_expression != value)
				{
					_expression = value;
					if (null != _expression)
					{
						_expression.InitializeParent(this);
					}
				}
			}

		}
		

	}
}
