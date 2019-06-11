<?php

namespace App\Http\Controllers\Api;



use App\Feedback;
use App\File;
use App\Http\Controllers\Controller;
use App\Story;
use App\StoryFeedback;
use Illuminate\Http\Request;
use Illuminate\Support\Carbon;
use Illuminate\Support\Facades\Storage;

class LabyrintApiController extends Controller
{


    public function documentation(){

        return file_get_contents("https://alwinkroesen.docs.apiary.io/")."
        <script>
            setTimeout(function(){
                if (window.history.replaceState) {
                   //prevents browser from storing history with each change:
                   window.history.replaceState(null, 'documentation', '/api/v1/');
                }
                var header = document.querySelector('#applicationHeader');
                    header.style.display = 'none';
                var h1 = document.createElement('h1');
                    h1.innerHTML = 'Documentation';
                    h1.style.color = '#FFF';
                    h1.style.paddingTop = '15px';
                    h1.style.paddingLeft = '20px';
                    
                    header.parentNode.appendChild(h1);
            },500);
        </script>
        ";

    }


    /**
     * Get the order of the storyItems with the feedback
     * @return \Illuminate\Http\JsonResponse
     */
    public function getOrder(){

        // Get the stories and feedback
        $stories = Story::with('feedback')->where('active', 1)->get();
        $feedbacks = Feedback::with('feedbackItems')->get();

        // Create a default array for the feedback for each story
        $fs = [];
        foreach ($feedbacks as $feedback){
            $fis = [];
            foreach ($feedback->feedbackItems as $fi){
                $fis[$fi->id] = [
                    'id' => $fi->id,
                    'count' => 0
                ];
            }
            $fs[$feedback->id] = [
                'id' => $feedback->id,
                'total' => 0,
                'items' => $fis
            ];
        }

        // Create a list of stories
        $a = [];
        foreach ($stories as $story){

            // Set default array in a temp variable
            $sfs = $fs;
            $total = 0;
            foreach ($story->feedback as $fi){
                // Calculate the amount of feedback given
                $sfs[$fi->question->id]['total']++;
                $sfs[$fi->question->id]['items'][$fi->id]['count']++;
            }

            foreach ($sfs as $k => $v){
                // Count totals
                $total += $sfs[$k]['total'];
            }

            // Set feedback
            $a[$story->id] = [
                'id' => $story->id,
                'total' => $total,
                'feedback' => $sfs
            ];
        }



        $order = [];
        $fbOrder = [];
        foreach ($fs as $f){
            $fbOrder[$f['id']] = [
                'id' => $f['id'],
                'count' => 0
            ];
        }

        // Return minimal 1000 stories
        for($i=0;$i<1000;){

            foreach ($a as $j => $story){


                $sfs = $story['feedback'];
                foreach ($sfs as $k => $v){
                    // Sort feedbackitems, lowest first
                    usort($v['items'], function ($a, $b) {
                        if($a['count'] == $b['count']) return 0;
                        return ($a['count'] < $b['count']) ? -1 : 1;
                    });
                    $sfs[$k]['items'] = $v['items'];
                }
                // Sort feedback, lowest first
                usort($sfs, function ($a, $b) {
                    if($a['total'] == $b['total']) return 0;
                    return ($a['total'] < $b['total']) ? -1 : 1;
                });
                // Set the sorted feedback
                $a[$j]['feedback'] = $sfs;
            }



            // Sort stories
            usort($a, function ($a, $b) {
                if($a['total'] == $b['total']) return 0;
                return ($a['total'] < $b['total']) ? -1 : 1;
            });


            foreach ($a as $k => $b){
                $i++;

                // Set the id of the less used feedback of this story
                $fid = $b['feedback'][0]['id'];

                // Set the used feedback
                $fbTempOrder = $fbOrder;

                // Order the most/less used feedback, lowest first
                usort($fbTempOrder, function($a, $b){
                    if($a['count'] == $b['count']) return 0;
                    return ($a['count'] < $b['count']) ? -1 : 1;
                });

                // Check so you don't have more than 3 times the same feedback
                if($fbTempOrder[0]['id'] != $fid){
                    if($fbTempOrder[0]['count']+2 < $fbOrder[$fid]['count']){
                        $fid = $fbTempOrder[0]['id'];
                    }
                }
                // Add a count to the used feedback
                $fbOrder[$fid]['count']++;

                // Add a count to the story total
                $a[$k]['total']++;

                $fbnum = -1; // feebackNum
                $fbinum = -1; // feedbackItemNum
                foreach ($b['feedback'] as $n => $f){
                    $fbnum = $n;
                    foreach ($f['items'] as $m => $fi){
                        if($fi['id'] == $fid){
                            $fbinum = $m;
                            break;
                        }
                    }
                    if($fbinum >= 0) break;
                }
                // Add count to the used feedbackItem for this story
                $a[$k]['feedback'][$fbnum]['items'][$fbinum]['count']++;

                // Put in order
                $order[] = [
                    'storyId' => $b['id'],
                    'feedbackId' => $fid
                ];
            }
        }

        return response()->json($order);
    }


    /**
     * Get feedback
     * @return \Illuminate\Http\JsonResponse
     */
    public function getFeedback(){

        $feedbacks = Feedback::with('feedbackItems')->get();

        $data = [];

        foreach ($feedbacks as $feedback){
            $answers = [];

            foreach ($feedback->feedbackItems as $feedbackItem){
                $answers[] = [
                    'answerId' => $feedbackItem->id,
                    'response' => $feedbackItem->feedback
                ];
            }

            $data[] = [
                'feedbackId' => $feedback->id,
                'question' => $feedback->question,
                'extraInfo' => $feedback->extraInfo,
                'feedbackType' => $feedback->feedbackType,
                'answers' => $answers
            ];
        }

        return  response()->json($data);
    }

    /**
     * Get statistics
     * @return \Illuminate\Http\JsonResponse
     */
    public function getStatistics(){

        // Get all feedbacks
        $feedbacks = Feedback::with('feedbackItems', 'feedbackItems.storyFeedback')->get();

        $data = [];
        foreach ($feedbacks as $feedback) {
            $feedbackItemData = [];
            $count = 0;
            // Set feedback item data
            foreach ($feedback->feedbackItems as $feedbackItem){

                // Get json data
                $feedbackItemJson = $feedbackItem->getJson();

                // Count given feedback
                $feedbackItemJson['count'] = $feedbackItem->storyFeedback->count();
                $count += $feedbackItemJson['count'];
                $feedbackItemData[] = $feedbackItemJson;
            }

            // Set feedback data
            $data[] = [
                'id' => $feedback->id,
                'question' => $feedback->question,
                'extraInfo' => $feedback->extraInfo,
                'feedbackType' => $feedback->feedbackType,
                'oneWord' => $feedback->oneWord,
                'count' => $count,
                'feedback' => $feedbackItemData
            ];
        }

        return  response()->json($data);
    }


    /**
     * Save feedback
     * @param Request $request
     * @return \Illuminate\Http\JsonResponse
     */
    public function returnFeedback(Request $request){
        $data = [];

        // Check if the data can be read as JSON, return error when can't
        if(!$request->isJson()){
            $data['response'] = 'failed';
            $data['errors'] = [
                'Not send as JSON'
            ];
            return  response()->json($data);
        }
        // Loop through all given feedback
        foreach ($request->json() as $feedback){

            // Check if each has an storyId and an answerId
            if(isset($feedback['storyId']) && isset($feedback['answerId'])){

                // Save feedback
                StoryFeedback::create([
                    'storyId' => $feedback['storyId'],
                    'feedbackId' => $feedback['answerId'],
                ]);
            }
        }

        // Return success
        $data['response'] = 'success';
        return  response()->json($data);
    }

    /**
     * Download a file
     * @param $fileId
     * @return \Illuminate\Http\JsonResponse|\Symfony\Component\HttpFoundation\BinaryFileResponse
     */
    public function downloadFile($fileId){

        // Get the file from the database
        $file = File::find($fileId);

        // Check if file is in the database
        if(!$file){
            return response()->json([
                'response' => 'failed',
                'errors' => [
                    'File not found'
                ]
            ]);
        }

        // Check if file exists
        if(!file_exists(storage_path($file->path.$file->fileName))){
            return response()->json([
                'response' => 'failed',
                'errors' => [
                    'File not found'
                ]
            ]);
        }

        // Return file to download
        return response()->download(storage_path($file->path.$file->fileName), $file->realName);

    }

    /**
     * Convert a file
     * @param $fileId
     * @return \Illuminate\Http\JsonResponse|\Symfony\Component\HttpFoundation\BinaryFileResponse
     */
    public function convertFile(Request $request){

        $file = $request->file('file');

        // Check if file is in the database
        if(!$file){
            return response()->json([
                'response' => 'failed',
                'errors' => [
                    'File not found'
                ]
            ]);
        }

        // Get file data
        $f = $file;
        $extension = $f->getClientOriginalExtension();
        $fn = $f->getClientOriginalName();

        // Create a new filename
        $filename = Carbon::now()->format('Ymdhis') . rand(11111111, 99999999) . '.' . $extension;

        // Save file
        $file->storeAs("/uploads/temp/", $filename, 'local');

        /** @var File $file */
        // Create file
        $file = File::make([
            'fileName' => $filename,
            'realName' => $fn,
            'fileType' => $f->getMimeType(),
            'extension' => $extension,
            'path' => 'app/uploads/temp/',
        ]);


        $text = "";

        // Image files
        if(in_array($extension, File::$allowedImageFiles)){
            $text = $file->convertImageFile();
        }
        // Word files
        if(in_array($extension, File::$allowedTextFiles)){
            // Set story item text if the file is an docx file
            $text = $file->convertDocxFile();
        }
        // Video files
        if(in_array($extension, File::$allowedVideoFiles)){
            // Set story item text if the file is an docx file
            $text = $file->convertVideoFile();
        }

        if(Storage::disk('local')->exists("/uploads/temp/".$filename)){
            Storage::disk('local')->delete("/uploads/temp/".$filename);
        }


        // Return file to download
        return response()->json([
            'response' => 'success',
            'data' => $text
        ]);

    }
}
