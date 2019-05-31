<?php

namespace App\Http\Controllers\Api;



use App\Feedback;
use App\File;
use App\Http\Controllers\Controller;
use App\Story;
use App\StoryItem;
use Carbon\Carbon;
use http\Env\Response;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Validator;
use phpDocumentor\Reflection\Types\Integer;
use Symfony\Component\HttpFoundation\File\UploadedFile;

class StoryApiController extends Controller
{


    /**
     * Page for the overview
     * @return \Illuminate\Http\JsonResponse
     */
    public function getOverview(){

        $data = [];

        // Get all stories
        $stories = \App\Story::with('feedback')->get();

        // Get all feedback items
        $feedbacks = \App\Feedback::with('feedbackItems')->get();

        // Loop through all stories
        foreach ($stories as $story){
//            $questions = [];
//            foreach ($feedbacks as $feedback){
//                $answers = [];
//                // add each feedbackitem to a story.
//                foreach ($feedback->feedbackItems as $feedbackItem){
//                    $answers[$feedbackItem->id] = [
//                        'feedbackId' => $feedbackItem->id,
//                        'answer' => $feedbackItem->feedback,
//                        'count' => 0,
//                    ];
//                }
//                // Count the amount of feedback given
//                foreach ($story->feedback as $feedbackItem){
//                    if($feedbackItem->question->id != $feedback->id) continue;
//                    if( isset($answers[$feedbackItem->id]) &&
//                        isset($answers[$feedbackItem->id]['count']) ){
//                        $answers[$feedbackItem->id]['count'] += 1;
//                    }
//                }
//                // Sort answers
//                $answers = array_values($answers);
//                usort($answers, function($a, $b) {
//                    return $a['count'] - $b['count'];
//                });
//
//                // Add feedback to the questions which will be added to the story
//                $questions[] = [
//                    'question' => $feedback->question,
//                    'extraInfo' => $feedback->extraInfo,
//                    'feedbackType' => $feedback->feedbackType,
//                    'answers' => array_values($answers)
//                ];
//            }
//            $questions =


            // add story to data
            $data[] = [
                'storyId' => $story->id,
                'title' => $story->title,
                'description' => $story->description,
                'icon' => $story->icon,
                'active' => $story->active,
                'feedback' => $story->allFeedback()
            ];

        }

        return  response()->json($data);
    }

    /**
     * Get all data of a single story
     * @param $storyId
     * @return \Illuminate\Http\JsonResponse
     */
    public function getStory($storyId){

        $data = [];

        // Get story
        $story = Story::with('storyItems', 'storyItems.file', 'feedback', 'feedback.question')->find($storyId);


        // Return failed response when story couldn't be found
        if(!$story){
            return response()->json([
                'response' => 'failed',
                'errors' => [
                    'Story not found'
                ]
            ]);
        }

        // Set data to return
        $data['id'] = $story->id;
        $data['title'] = $story->title;
        $data['icon'] = $story->icon;
        $data['description'] = $story->description;
        $data['active'] = $story->active ? true : false;
        $data['texts'] = [];

        $host = request()->getHttpHost();
        if(substr($host, 0, 4) != 'http'){
            if(substr(url()->current(), 0,5) == 'https'){
                $host = "https://" . $host;
            }else{
                $host = "http://" . $host;
            }
        }

        // Add story items
        foreach ($story->storyItems as $storyItem){
            $file = null;
            if($storyItem->file){
                $f = $storyItem->file;
                // add files
                $file = [
                    'id' => $f->id,
                    'filename' => $f->fileName,
                    'realname' => $f->realName,
                    'path' => $f->path,
                    'fileType' => $f->fileType,
                    'download' => $host.'/api/v1/file/'.$f->id // Download file
                ];
            }
            $data['texts'][] = [
                'id' => $storyItem->id,
                'text' => $storyItem->text,
                'file' => $file
            ];
        }

        // Add feedback
        $data['feedbackItems'] = $story->allFeedback();

        return response()->json($data);
    }

    /**
     * Get a list of active stories
     * @return \Illuminate\Http\JsonResponse
     * @throws \Throwable
     */
    public function getStories(){

        $data = [];

        // Get all active stories
        $stories = Story::with('storyItems')->where('active', 1)->get();

        // Set the correct data
        foreach ($stories as $story){
            $data[] = [
                'storyId' => $story->id,
                'icon' => $story->icon,
                'html' => view('information-piece', compact('story'))->render()
            ];
        }
        return response()->json($data);
    }

    /**
     * Create a new story
     * @param Request $request
     * @return \Illuminate\Http\JsonResponse
     */
    public function newStory(Request $request){

        // Get the post variables
        $title = $request->get('title');
        $icon = $request->get('icon');
        $description = $request->get('description');
        $texts = $request->get('texts');
        $files = $request->file('files');


        // Validate
        $validation = Validator::make($request->all(), [
            'title' => 'required',
            'icon' => 'required'
        ]);

        // When validation fails, return error
        if ($validation->fails()) {
            return response()->json([
                'response' => 'failed',
                'errors' => $validation->errors()
            ]);
        }

        // Set story data
        $storyData = [
            'title' => $title,
            'icon' => $icon
        ];

        // When there is a description, add it.
        if(strlen($description) > 0){
            $storyData['description'] = $description;
        }


        // Create story
        $story = Story::create($storyData);

        // Add texts
        if($texts) {
            if(!is_array($texts)){
                $texts = [$texts];
            }
            foreach ($texts as $text) {
                $storyItem = StoryItem::create([
                    'text' => $text,
                    'storyId' => $story->id
                ]);
            }
        }
        // Add files
        if($files){

            foreach ($files as $file) {

                // Get file data
                $f = $file;
                $extension = $f->getClientOriginalExtension();
                $fn = $f->getClientOriginalName();

                // Create a new filename
                $filename = Carbon::now()->format('Ymdhis') . rand(11111111, 99999999) . '.' . $extension;

                // Save file
                $file->storeAs("/uploads/story/", $filename, 'local');

                // Create storyItem for this file
                $storyItem = StoryItem::create([
                    'text' => null,
                    'storyId' => $story->id
                ]);

                // Create file
                $file = File::create([
                    'fileName' => $filename,
                    'realName' => $fn,
                    'fileType' => $f->getMimeType(),
                    'extension' => $extension,
                    'path' => 'app/uploads/story/',
                    'storyItemId' => $storyItem->id
                ]);

                // Set story item text if the file is an docx file
                $text = $this->convertDocxFile($file);
                if($text != null){
                    $storyItem->update(['text' => $text]);
                }
            }
        }

        // Return success
        return response()->json([
            'response' => 'success',
            'storyId' => $story->id
        ]);

    }

    public function changeStory(Request $request, $storyId){

        $story = Story::with('storyItems', 'storyItems.file')->find($storyId);
        if(!$story){
            return response()->json([
                'response' => 'failed',
                'errors' => [
                    'Story not found'
                ]
            ]);
        }

        $newData = [];

        if($request->has('title')){
            $newData['title'] = $request->get('title');
        }
        if($request->has('icon')){
            $newData['icon'] = $request->get('icon');
        }
        if($request->has('description')){
            $newData['description'] = $request->get('description');
        }

        // TODO change texts and files


        if($request->has('texts')){
            $texts = $request->get('texts');
            foreach ($story->storyItems as $storyItem){
                if(isset($texts[$storyItem->id])){
                    $storyItem->text = $texts[$storyItem->id];
                    $storyItem->update();
                }
            }
        }
        if($request->has('newTexts')){
            foreach ($request->get('newTexts') as $text){
                if(strlen($texts) == 0) continue;
                StoryItem::create([
                    'storyId' => $story->id,
                    'text' => $text
                ]);
            }
        }

        // Add files
        if($request->has('files')){
            $files = $request->file('files');
            if($files){

                foreach ($files as $file) {

                    // Get file data
                    $f = $file;
                    $extension = $f->getClientOriginalExtension();
                    $fn = $f->getClientOriginalName();

                    // Create a new filename
                    $filename = Carbon::now()->format('Ymdhis') . rand(11111111, 99999999) . '.' . $extension;

                    // Save file
                    $file->storeAs("/uploads/story/", $filename, 'local');

                    // Create storyItem for this file
                    $storyItem = StoryItem::create([
                        'text' => null,
                        'storyId' => $story->id
                    ]);

                    // Create file
                    $file = File::create([
                        'fileName' => $filename,
                        'realName' => $fn,
                        'fileType' => $f->getMimeType(),
                        'extension' => $extension,
                        'path' => 'app/uploads/story/',
                        'storyItemId' => $storyItem->id
                    ]);

                    // Set story item text if the file is an docx file
                    $text = $this->convertDocxFile($file);
                    if($text != null){
                        $storyItem->update(['text' => $text]);
                    }
                }
            }
        }
        $story->update();


        return response()->json([
            'response' => 'success'
        ]);
    }


    /**
     * Delete a story
     * @param $storyId
     * @return \Illuminate\Http\JsonResponse
     * @throws \Exception
     */
    public function deleteStory($storyId){

        // Get story
        $story = Story::with('storyItems', 'storyItems.file')->find($storyId);

        //
        if(!$story){

            return response()->json([
                'response' => 'failed',
                'errors' => [
                    'Story not found'
                ]
            ]);
        }

        // Remove files of the story
        foreach ($story->storyItems as $storyItem){

            $this->deleteStoryItem($storyItem->id);
        }

        // Delete story
        $story->delete();


        return response()->json([
            'response' => 'success',
            'storyId' => $storyId
        ]);
    }

    /**
     * Delete a story item with file
     * @param $storyItemId
     * @return \Illuminate\Http\JsonResponse
     * @throws \Exception
     */
    public function deleteStoryItem($storyItemId){

        // Get StoryItem
        $storyItem = StoryItem::with('file')->find($storyItemId);

        if(!$storyItem){

            return response()->json([
                'response' => 'failed',
                'errors' => [
                    'StoryItem not found'
                ]
            ]);
        }

        // Remove file
        if(isset($storyItem->file)){
            $file = storage_path($storyItem->file->path.$storyItem->file->fileName);
            if(file_exists($file)){
                unlink($file);
            }
        }

        // Remove item
        $storyItem->delete();

        return response()->json([
            'response' => 'success',
            'storyItemId' => $storyItemId
        ]);
    }

    /**
     * Set a story on active or nonactive
     * @param $storyId
     * @param $active "activate"|"deactivate"
     * @return \Illuminate\Http\JsonResponse
     */
    public function storyChangeActive($storyId, $active){


        // Get the story
        $story = Story::find($storyId);

        // Return error when story couldn't be found
        if(!$story){
            return response()->json([
                'response' => 'failed',
                'errors' => [
                    'Story not found'
                ]
            ]);
        }

        // Update story
        $story->update([
            'active' => $active == 'activate' ? 1 : 0
        ]);

        // Return success
        return response()->json([
            'response' => 'success'
        ]);
    }

    /**
     * Convert a docx file to text
     * @param File $file
     * @return bool|string|null
     */
    public function convertDocxFile(File $file){

        // Check if it is a docx file
        if($file->extension != 'docx') return null;
        $content = '';

        // Open file as a zip file
        $zip = zip_open(storage_path($file->path.$file->fileName));

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

        // Find drawing tag
        foreach ($v->r as $l => $w) {
            if(isset($w->drawing)){
                return $w->drawing;
            }
            if(count($w) > 0){
                $d = $this->findDrawing($w);
                if($d != null) return $d;
            }
        }
        return null;
    }


}
