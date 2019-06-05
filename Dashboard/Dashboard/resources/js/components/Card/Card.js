import React, { useEffect, useState } from 'react';
import HSBar from 'react-horizontal-stacked-bar-chart';
import toastr from 'toastr';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faTrashAlt } from '@fortawesome/free-solid-svg-icons'

const card = (props) => {
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

 function generateValues(toDataArray){
  var data = [];
  var colors = ["blue","red","yellow","green"];

  {toDataArray.map((innerFeedbackValue, innerFeedbackIndex) =>
    data.push({value: innerFeedbackValue['count'], description: innerFeedbackValue['count'], color: colors[innerFeedbackIndex]})
  )}

  return data; 
 }

  return (        
    <div className="col s12 m4">
      <div className="card">
         <div className="card-content white-text">
          <div className="row adminPanel">
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
          <div className="row divide">
            <div className="col s12">
              <ul>
                <li className="divider"></li>
              </ul>
            </div>
          </div>
          <div className="row cardTitle">
            <div className="col s12">
               <span className="card-title">{props.title}</span>
            </div>
          </div>
          <div className="row feedback">
            {props.cardInfo.map((cardValue, cardIndex) =>
              <span key={cardIndex}>
                {console.log(props.cardInfo)}
              {cardValue[cardIndex]['feedback'].map((OuterFeedbackValue, OuterFeedbackIndex) =>
                <div key={OuterFeedbackIndex}>
                  <div className="col s3">
                    <p>{OuterFeedbackValue['oneWord']}</p>
                  </div>
                  <div className="col s9">
                    <div className="bar">
                      <HSBar
                        data={generateValues((OuterFeedbackValue['feedback']))}
                      />
                    </div>
                  </div>
                </div>
              )}
              </span>         
            )}
          </div>
         </div>
      </div>
    </div>
  )


};

export default card;