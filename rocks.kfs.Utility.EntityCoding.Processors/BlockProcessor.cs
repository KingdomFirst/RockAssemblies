// <copyright>
// Copyright 2023 by Kingdom First Solutions
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.Linq;

using Rock.Data;
using Rock.Model;
using Rock.Utility.EntityCoding;

namespace rocks.kfs.Utility.EntityCoding.Processors
{
    /// <summary>
    /// Handles processing of Block entities.
    /// </summary>
    public class BlockProcessor : EntityProcessor<Block>
    {
        /// <summary>
        /// The unique identifier for this entity processor. This is used to identify the correct
        /// processor to use when importing so we match the one used during export.
        /// </summary>
        public override Guid Identifier { get { return new Guid( "02E3174D-C62A-4BA0-843F-A5D27D86645D" ); } }

        /// <summary>
        /// Evaluate the list of referenced entities. This is a list of key value pairs that identify
        /// the property that the reference came from as well as the referenced entity itself. Implementations
        /// of this method may add or remove from this list. For example, an AttributeValue has
        /// the entity it is referencing in a EntityId column, but there is no general use information for
        /// what kind of entity it is. The processor can provide that information.
        /// </summary>
        /// <param name="entity">The parent entity of the references.</param>
        /// <param name="references"></param>
        /// <param name="helper">The helper class for this export.</param>
        protected override void EvaluateReferencedEntities( Block entity, List<KeyValuePair<string, IEntity>> references, EntityCoder helper )
        {
            //
            // HTML Content Blocks have a specific way the content is referenced, but not actually linked to a Block Entity.
            // We go ahead and stand up a custom reference to the HTMLContent entity so that the data can be exported appropriately.
            //
            if ( entity.BlockTypeId == 6 )
            {
                var rockContext = new RockContext();
                var htmlContentService = new HtmlContentService( rockContext );
                var htmlContent = htmlContentService.GetByBlockId( entity.Id );

                if ( htmlContent.Any() )
                {
                    foreach ( var blockHtmlContent in htmlContent )
                    {
                        references.Add( new KeyValuePair<string, IEntity>( "HtmlContents", blockHtmlContent ) );
                    }
                }
            }

        }
    }
}
