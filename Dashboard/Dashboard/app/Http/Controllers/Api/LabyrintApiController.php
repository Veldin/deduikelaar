<?php

namespace App\Http\Controllers\Api;



use App\Feedback;
use App\Http\Controllers\Controller;
use App\Story;

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

    public function getStatistics(){
        if(isset($_GET['onlyActive'])){
            $stories = \App\Story::with('feedback')->where('active', 1)->get();
        }else{
            $stories = \App\Story::with('feedback')->get();
        }
        $data = [];
        foreach ($stories as $story){
            $feedback = [];
            foreach ($story->feedback as $feedbackItem){
                if(isset($feedback[$feedbackItem->id])){
                    $feedback[$feedbackItem->id]['count']++;
                }else{
                    $feedback[$feedbackItem->id] = [
                        'feedbackId' => $feedbackItem->id,
                        'answer' => $feedbackItem->feedback,
                        'count' => 1
                    ];
                }

            }
            $data[] = [
                'storyId' => $story->id,
                'title' => $story->title,
                'active' => $story->active,
                'feedback' => $feedback
            ];
        }

        return  response()->json($data);
    }


}
