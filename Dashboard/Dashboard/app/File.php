<?php

namespace App;

use Illuminate\Database\Eloquent\Model;

class File extends Model
{
    /**
     * The table associated with the model.
     *
     * @var string
     */
    protected $table = 'file';

    // All allowed file extensions
    public static $allowedExtensions = ['docx', 'pdf','jpeg','jpg','png','gif','bmp','avi','mp4','mpeg', 'webm', 'mp3','wav'];

    // Image file extensions
    public static $imageFileExtensions = ['jpeg','jpg','png','gif','bmp'];

    // Video file extensions
    public static $videoFilesExtensions = ['avi','mp4','mpeg', 'webm'];

    // Audio file extensions
    public static $audioFilesExtensions = ['mp3','wav'];

    // Fields from the database
    protected $fillable = [
        'fileName',
        'realName',
        'fileType',
        'extension',
        'path',
        'storyItemId'
    ];

    // Get the storyItem
    public function storyItem() {
        return $this->belongsTo(StoryItem::class,'storyItemId','id');
    }

    /**
     * Get the file as text (html)
     * @return false|string|null
     */
    public function getFileAsText(){
        $text = "";
        // Image files
        if(in_array($this->extension, self::$imageFileExtensions)){
            $text = $this->convertImageFile();
        }
        // Word files
        if($this->extension == 'docx'){
            // Set story item text if the file is an docx file
            $text = $this->convertDocxFile();
        }
        // PDF files
        if($this->extension == 'pdf'){
            // Set story item text if the file is an pdf file
            $text = $this->convertPDFFile();
        }
        // Video files
        if(in_array($this->extension, self::$videoFilesExtensions)){
            // Set story item text if the file is an video file
            $text = $this->convertVideoFile();
        }
        // Audio files
        if(in_array($this->extension, self::$audioFilesExtensions)){
            // Set story item text if the file is an video file
            $text = $this->convertAudioFile();
        }
        return $text;
    }

    /**
     * Convert an image to html tag
     * @return string
     */
    private function convertImageFile(){

        // Check if it is an image file
        if(!in_array($this->extension, self::$imageFileExtensions)) return "";

        // Get file location
        $path = storage_path($this->path.$this->fileName);

        // If file not exists, return empty string.
        if(!file_exists($path)) return "";


        $type = pathinfo($path, PATHINFO_EXTENSION); // Get file type
        $data = file_get_contents($path); // Get the file content
        $base64 = 'data:image/' . $type . ';base64,' . base64_encode($data); // Convert file content to base64

        // Return string(html)
        return "<img src='".$base64."' alt='".$this->realName."'>";
    }

    private function convertVideoFile(){

        // Check if it is an video file
        if(!in_array($this->extension, self::$videoFilesExtensions)) return "";

        // Get file location
        $path = storage_path($this->path.$this->fileName);

        // If file not exists, return empty string.
        if(!file_exists($path)) return "";


        $type = pathinfo($path, PATHINFO_EXTENSION); // Get file type
        $data = file_get_contents($path); // Get the file content
        $base64 = 'data:video/' . $type . ';base64,' . base64_encode($data); // Convert file content to base64

        // Return string(html)
        return "<video controls><source type=\"video/".$type."\" src=\"".$base64."\"></video>";
    }

    private function convertAudioFile(){

        // Check if it is an audio file
        if(!in_array($this->extension, self::$audioFilesExtensions)) return "";

        // Get file location
        $path = storage_path($this->path.$this->fileName);

        // If file not exists, return empty string.
        if(!file_exists($path)) return "";


        $type = pathinfo($path, PATHINFO_EXTENSION); // Get file type
        $data = file_get_contents($path); // Get the file content
        $base64 = 'data:audio/' . $type . ';base64,' . base64_encode($data); // Convert file content to base64

        // Return string(html)
        return "<audio controls><source type=\"audio/".$type."\" src=\"".$base64."\"></audio>";
    }

    private function convertPDFFile(){

        // Check if it is an pdf file
        if($this->extension != 'pdf') return "";

        // Check if the computer where php is running on, is a windows computer
        if(strtoupper(substr(PHP_OS, 0, 3)) !== 'WIN') return "";

        // Get the pdf location
        $source_pdf = storage_path($this->path.$this->fileName);

        // Set the output folder
        $output_folder = storage_path("app/uploads/temp");

        // Create output folder if it doesn't exists
        if(!file_exists($output_folder)) mkdir($output_folder, 777);

        // Command to convert pdf to html with 'pdftohtml.exe'
        $cmd = '"'.storage_path().'\\app\\pdftohtml.exe" "'.$source_pdf.'" "'.$output_folder.'\\'.$this->fileName.'"';

        // Execute command
        exec( $cmd, $out, $ret);

        // Set empty data when execution didn't go well
        $data = "";

        // Execution was successful
        if(file_exists($output_folder.'\\'.$this->fileName.'s.html')){
            // Get the html data
            $data = file_get_contents($output_folder.'\\'.$this->fileName.'s.html');

            // Find and replace image tags.
            $data = preg_replace_callback ('/src="(.*?)"/i', function($matches) use ($output_folder) {
                // Get the image location
                $file = $output_folder."\\".$matches[1];

                // Get the image info
                $type = pathinfo($file, PATHINFO_EXTENSION);

                // Get the image content
                $data = file_get_contents($file);

                // Convert image content to base64 and return it
                return 'src="data:image/' . $type . ';base64,' . base64_encode($data).'"';
            }, $data);
        }

        // Get files only with this filename
        $files = glob($output_folder.'/'.$this->fileName.'*');

        foreach($files as $file){ // iterate files
            if(is_file($file))
                unlink($file); // delete file
        }

        // Return the data
        return $data;
    }


    /**
     * Convert a docx file to text
     * @return bool|string|null
     */
    private function convertDocxFile(){

        // Check if it is a docx file
        if($this->extension != 'docx') return "";
        $content = '';

        // Open file as a zip file
        $zip = zip_open(storage_path($this->path.$this->fileName));

        // Check if it is an zip file
        if (!$zip || is_numeric($zip)) return null;
        $media = [];
        $relations = "";

        // Loop through files
        while ($zip_entry = zip_read($zip)) {

            if (zip_entry_open($zip, $zip_entry) == FALSE) continue;

            // Set relations
            if(substr(zip_entry_name($zip_entry), 0, 11) == "word/_rels/"){
                $relations = zip_entry_read($zip_entry, zip_entry_filesize($zip_entry));
            }

            // Set media
            if(substr(zip_entry_name($zip_entry), 0,11) == "word/media/"){

                $media[substr(zip_entry_name($zip_entry), 5)] = [
                    'size' => zip_entry_filesize($zip_entry),
                    'data' => base64_encode(zip_entry_read($zip_entry, zip_entry_filesize($zip_entry)))
                ];
            }

            // Ignore all other files, except for the actual document
            if (zip_entry_name($zip_entry) != "word/document.xml") continue;

            // Set the content
            $content .= zip_entry_read($zip_entry, zip_entry_filesize($zip_entry));

            zip_entry_close($zip_entry);
        }// end while

        zip_close($zip);


        // Read relations as XML
        $xmlRel = simplexml_load_string($relations);

        // Change media array to key -> value where key is the id from relations file
        foreach ($xmlRel->Relationship as $relation) {
            $attributes = $relation->attributes();
            if(strpos((string) $attributes->Type, 'image') !== false){
                $media[(string)$attributes->Id] = $media[(string)$attributes->Target];
                unset($media[(string)$attributes->Target]);
            }
        }

        // Read document as XML
        $xml = simplexml_load_string($content,null, 0, 'w', true);

        // find all namespaces
        preg_match_all('/xmlns:(.*?)="(.*?)"/i', $content, $matches);

        // Add all the namespaces to the xml reader
        $namespaces = [];
        if(count($matches) > 2){
            foreach ($matches[1] as $i => $v){
                if(isset($matches[2][$i])){
                    $xml->registerXPathNamespace($v, $matches[2][$i]);
                    $namespaces[$v] = true;
                }
            }
        }

        // Document body
        $body = $xml->body;

        $content = "";
        $mediaIndex = 0;
        // Loop through each tag that is directly in the body tag
        foreach($body[0] as $key => $value){

            // Open html paragraph
            $content .= "<p>";
            if($key == "p"){

                // Set text
                foreach ($value->r as $kkey => $vvalue) {
                    $content .= (string)$vvalue->t;
                }

                // Check if there are images
                $drawing = $this->findDrawing($value);
                if($drawing){
                    // Find image id
                    $search = $drawing->xpath("//*[local-name()='blip']");
                    if(isset($search[$mediaIndex])){
                        if(isset($search[$mediaIndex]->attributes('r', true)[0])){
                            $id = (string) $search[$mediaIndex]->attributes('r', true)[0];
                            if(isset($media[$id])){
                                // image id found
                                // Add image tag with base64 source
                                $content .= "<img src='data:image/png;base64, ".$media[$id]['data']."'/>";
                            }
                            $mediaIndex++;
                        }
                    }
                }
            }

            // End html paragraph
            $content .= "</p>";
        }

        return $content;

    }


    /**
     * Find drawing tag for image
     * @param $v \SimpleXMLElement
     * @return \SimpleXMLElement|null
     */
    private function findDrawing($v){

        // Search for drawing tag
        foreach ($v->r as $l => $w) {
            if(isset($w->drawing)){
                return $w->drawing; // drawing tag was found
            }
            // If tag has children, go search deeper
            if(count($w) > 0){
                $d = $this->findDrawing($w); // Search deeper
                if($d != null) return $d; // Return drawing
            }
        }
        // Drawing not found, return null
        return null;
    }



}