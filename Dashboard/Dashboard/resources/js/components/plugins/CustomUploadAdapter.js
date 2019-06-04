/**
 * @license Copyright (c) 2003-2018, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md.
 */

import ClassicEditor from '@ckeditor/ckeditor5-build-classic';


class CustomUploadAdapter {

    constructor( loader ) {
        // The file loader instance to use during the upload. It sounds scary but do not
        // worry — the loader will be passed into the adapter later on in this guide.
        this.loader = loader;
    }


    // Starts the upload process.
    upload() {
        return this.loader.file
            .then( file => new Promise( ( resolve, reject ) => {
                var reader = new FileReader();
                reader.onload = function(event){
                    resolve( {
                        default: event.target.result
                    } );
                };
                reader.readAsDataURL(file);
            } ) );
    }
}

export default CustomUploadAdapter;

