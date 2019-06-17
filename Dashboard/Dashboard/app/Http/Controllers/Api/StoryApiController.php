<?php

namespace App\Http\Controllers\Api;



use App\Feedback;
use App\File;
use App\Http\Controllers\Controller;
use App\Story;
use App\StoryFeedback;
use App\StoryItem;
use Carbon\Carbon;
use http\Env\Response;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;
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

        // Set default order
        $order = ['created_at' => 'DESC'];

        // Check if an order is given with the request
        if(isset($_GET['order'])){
            // Split the different orders
            $orders = explode(",", $_GET['order']);
            $order = [];
            // Loop through order options
            foreach ($orders as $v){
                $a = explode(":", $v);
                if(count($a) == 2){
                    $order[$a[0]] = $a[1];
                }else{
                    $order[$a[0]] = 'ASC';
                }
            }
        }

        // Start creating story query
        if(isset($_GET['onlyActive'])){
            $stories = Story::with('storyFeedback')->where('active', 1);
        }else{
            $stories = Story::with('storyFeedback');
        }
        // Set ordered by
        foreach ($order as $k => $v){
            $stories->orderBy($k, $v);
        }

        // Get all stories
        $stories = $stories->get();

        // Get number of story feedback
        $storyFeedback = StoryFeedback::getCount();

        // Get all stories with feedback items
        $feedbacks = Feedback::with('feedbackItems')->get();

        // Loop through all stories
        foreach ($stories as $story){
            // add story to data
            $data[] = [
                'storyId' => $story->id,
                'title' => $story->title,
                'description' => $story->description,
                'icon' => $story->icon,
                'active' => $story->active,
                'feedback' => $story->allFeedback($feedbacks, $storyFeedback)
            ];

        }

        // Return json data
        return response()->json($data);
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

        // Get host and set http or https
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
                // Add files to data
                $file = [
                    'id' => $f->id,
                    'filename' => $f->fileName,
                    'realname' => $f->realName,
                    'path' => $f->path,
                    'fileType' => $f->fileType,
                    'download' => $host.'/api/v1/file/'.$f->id // Download link for file
                ];
            }
            // Add texts to data
            $data['texts'][] = [
                'id' => $storyItem->id,
                'text' => $storyItem->text,
                'file' => $file
            ];
        }

        // Add feedback
        $data['feedbackItems'] = $story->allFeedback(null, StoryFeedback::getCount($story));


        // Return JSON data
        return response()->json($data);
    }

    /**
     * Get a list of active stories
     * @return \Illuminate\Http\JsonResponse
     * @throws \Throwable
     */
    public function getStories(){

        // Set max execution time to 5 minutes
        ini_set('max_execution_time', 300);
        $data = [];

        // Get all active stories
        $stories = Story::with('storyItems')->where('active', 1)->get();

        // Set the story data for the Labyrint
        foreach ($stories as $story){
            $data[] = [
                'storyId' => $story->id,
                'icon' => $story->icon,
                'html' => view('information-piece', compact('story'))->render()
            ];
        }

        // Return JSON data
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


        // Validate request
        $validation = Validator::make($request->all(), [
            'title' => 'required',
            'icon' => 'required',
            'files.*' => 'mimes:'.join(',',File::$allowedExtensions)
        ]);

        // When validation fails, return error
        if ($validation->fails()) {
            $errors = 0;

            // Ignore error 'Failed to upload'
            foreach ($validation->errors()->toArray() as $error){
                foreach ($error as $msg){
                    if(strpos($msg, 'failed to upload') == false){
                        $errors++;
                    }
                }
            }

            // Return errors when having errors
            if($errors){
                return response()->json([
                    'response' => 'failed',
                    'errors' => $validation->errors()
                ]);
            }

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
                StoryItem::create([
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

                /** @var File $file */

                $text = $file->getFileAsText();


                if($text != ""){
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

    /**
     * Change a story
     * @param Request $request
     * @param $storyId
     * @return \Illuminate\Http\JsonResponse
     */
    public function changeStory(Request $request, $storyId){

        // Get story
        $story = Story::with('storyItems', 'storyItems.file')->find($storyId);
        if(!$story){
            // Return error when
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
                if(strlen($text) == 0) continue;
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

                    /** @var File $file */
                    // Create file
                    $file = File::create([
                        'fileName' => $filename,
                        'realName' => $fn,
                        'fileType' => $f->getMimeType(),
                        'extension' => $extension,
                        'path' => 'app/uploads/story/',
                        'storyItemId' => $storyItem->id
                    ]);


                    $text = $file->getFileAsText();

                    if($text != ""){
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
     * Preview of a story
     * @param $storyId
     * @return \Illuminate\Http\JsonResponse
     * @throws \Exception
     */
    public function previewStory($storyId){

        $story = \App\Story::with('storyItems')->find($storyId);

        if(!$story){
            return "Informatiestuk niet gevonden.";
//            return response()->json([
//                'response' => 'failed',
//                'errors' => [
//                    'Story not found'
//                ]
//            ]);
        }
        return view('information-piece', compact('story'));
//
//        return response()->json([
//            'response' => 'success',
//            'data' => view('information-piece', compact('story'))->render()
//        ]);
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



}
