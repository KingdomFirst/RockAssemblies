// <copyright>
// Copyright 2026 by Kingdom First Solutions
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
import * as zlib from "zlib";

/**
 * Reads the text cells of an .xlsx workbook (its shared strings), without adding a
 * spreadsheet library: an .xlsx file is a zip archive and the strings live in
 * xl/sharedStrings.xml. Enough to check an export contains the expected names.
 */
export function readXlsxStrings( workbook: Buffer ): string[] {
    const xml = readZipEntry( workbook, "xl/sharedStrings.xml" );
    if ( xml === null ) {
        return [];
    }

    return [ ...xml.matchAll( /<t(?:\s[^>]*)?>([^<]*)<\/t>/g ) ].map( match => decodeXml( match[ 1 ] ) );
}

/** Finds an entry through the zip's central directory and returns it as UTF-8 text. */
function readZipEntry( zip: Buffer, name: string ): string | null {
    // End of central directory record: signature 0x06054b50, within the last 64 KB.
    let end = zip.length - 22;
    while ( end >= 0 && zip.readUInt32LE( end ) !== 0x06054b50 ) {
        end--;
    }
    if ( end < 0 ) {
        throw new Error( "Not a zip archive." );
    }

    const entryCount = zip.readUInt16LE( end + 10 );
    let offset = zip.readUInt32LE( end + 16 );

    for ( let i = 0; i < entryCount; i++ ) {
        const method = zip.readUInt16LE( offset + 10 );
        const compressedSize = zip.readUInt32LE( offset + 20 );
        const nameLength = zip.readUInt16LE( offset + 28 );
        const extraLength = zip.readUInt16LE( offset + 30 );
        const commentLength = zip.readUInt16LE( offset + 32 );
        const localHeader = zip.readUInt32LE( offset + 42 );
        const entryName = zip.toString( "utf8", offset + 46, offset + 46 + nameLength );

        if ( entryName === name ) {
            const dataStart = localHeader + 30 + zip.readUInt16LE( localHeader + 26 ) + zip.readUInt16LE( localHeader + 28 );
            const data = zip.subarray( dataStart, dataStart + compressedSize );
            return ( method === 0 ? data : zlib.inflateRawSync( data ) ).toString( "utf8" );
        }

        offset += 46 + nameLength + extraLength + commentLength;
    }

    return null;
}

function decodeXml( text: string ): string {
    return text
        .replace( /&lt;/g, "<" )
        .replace( /&gt;/g, ">" )
        .replace( /&quot;/g, "\"" )
        .replace( /&apos;/g, "'" )
        .replace( /&amp;/g, "&" );
}
