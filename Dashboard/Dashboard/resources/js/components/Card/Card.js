import React, { useEffect, useState } from 'react';
import HSBar from 'react-horizontal-stacked-bar-chart';
import toastr from 'toastr';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faTrashAlt } from '@fortawesome/free-solid-svg-icons'
import { faSmile } from '@fortawesome/free-solid-svg-icons'
import { faSadTear } from '@fortawesome/free-solid-svg-icons'
import { faAngry } from '@fortawesome/free-solid-svg-icons'

const card = (props) => {
  // const total = parseInt(props.emoteOne) + parseInt(props.emoteTwo) + parseInt(props.emoteThree);

  // const percentageEmoteOne = Math.round((props.emoteOne / total) * 100);
  // const percentageEmoteTwo = Math.round((props.emoteTwo / total) * 100);
  // const percentageEmoteThree = Math.round((props.emoteThree / total) * 100);

  // const percentageEmoteOneDOM = {
  //   width: percentageEmoteOne
  // };

  // const percentageEmoteTwoDOM = {
  //   width: percentageEmoteTwo
  // };

  // const percentageEmoteThreeDOM = {
  //   width: percentageEmoteThree
  // };

  function deleteItem() {
    fetch('/api/v1/story/'+props.storyID,{
      method: 'DELETE',
    })
    .then(response => response.json())
    .then(response => {
      if(response['response'] == "success"){
        toastr.success('dit item is verwijderd', '', {positionClass: "toast-bottom-right", timeOut: 40000})
        window.location.href = "/overview";
      }else{
        toastr.warning('dit item is al verwijderd', '', {positionClass: "toast-bottom-right", timeOut: 40000})
      }
    })
  }

  return (        
    <div className="col s12 m4">
      <div className="card">
         <div className="card-content white-text">
          <div className="row">
            <div className="col s10">
              <div className="switch">
                <label>
                  Actief
                  <input type="checkbox" defaultChecked></input> 
                  <span className="lever"></span>
                  Inactief
                </label>
              </div>
            </div>
            <div className="col s2">
              <div className="deleteItem" onClick={deleteItem}>
                <FontAwesomeIcon icon={faTrashAlt} />
              </div>
            </div>
          </div>
          <div className="row">
            <div className="col s12">
               <span className="card-title">{props.title}</span>
            </div>
          </div>
          <div className="row feedback">
            {props.cardInfo.map((cardValue, cardIndex) =>
              //console.log(item[key]['feedback'])
              {cardValue[cardIndex]['feedback'].map((OuterFeedbackValue, OuterFeedbackIndex) =>
                <div>
                  <div className="col s3">
                    <p>{OuterFeedbackValue['question']}</p>
                  </div>
                  <div className="col s9">
                    <div className="bar">
                     <HSBar
                      data={[
                        { value: 10, description: 'oke', color: "#ff0043" },
                        { value: 4, description: 'oke', color: "#77c6a0" },
                        { value: 8, description: 'oke', color: "#8391a5" }
                      ]}
                    />
                    </div>
                  </div>
                </div>
              )}            
            )}
          </div>
         </div>
      </div>
    </div>
  )


};

export default card;