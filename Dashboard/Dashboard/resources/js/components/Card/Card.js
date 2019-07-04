import React, { useEffect, useState } from 'react';
import HSBar from 'react-horizontal-stacked-bar-chart';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faTrashAlt, faEye } from '@fortawesome/free-solid-svg-icons';

// Hook to load in the cards
const card = (props) => {
  // Declare variables that access methods in the overview class using dependency injection
  const deleteItem = () => props.onDelete(props.storyID);
  const showItem = () => props.onShow(props.storyID);

  // Creates a feedback bar for each feedbackType in a story
  function createBarData(toDataArray){
    var data = [];

    // Declare the colours the bar uses
    var colors = ["#009688","#84c7c1","#ff0043","#e7edf2"];

    // Data to loop through
    var toloop = toDataArray.cardInfo[0];
    toloop.forEach(function(element) {
      // Check if feedback matches the correct story by comparing IDs
      if(toDataArray.storyID == element.storyId){
        element.feedback.forEach(function(feedbackItem) {
          // If oneWord is undefined, feedbackType doesn't exist
          if (typeof data[feedbackItem.oneWord] === 'undefined') {
            data[feedbackItem.oneWord] = [];
          }
            Object.entries(feedbackItem.feedback).map((element, index) =>
                {if(element[1].answer === "\\u1F603"){
                    element[1].answer = "😃";
                }else if(element[1].answer === "\\u1F620"){
                    element[1].answer = "😠";
                }else if(element[1].answer === "\\u1F622"){
                    element[1].answer = "😢";
                }}
            );

          // Loop through data and set each feedbackType with its data
          Object.entries(feedbackItem.feedback).map((element, index) =>
              data[feedbackItem.oneWord][index] ={value: element[1].count, description: element[1].answer, color: colors[index]}
          );
        });
      }
    });
    
    // Returns all the loaded data of feedback
    return data;
  }

  // Adds/Removes class based on switch state of card
  function toggleSwitch(){
    var card = document.getElementById(props.storyID);
    if(card.firstChild.classList.contains('active')){
      card.firstChild.classList.add("notActive")
      card.firstChild.classList.remove("active")
    }else{
      card.firstChild.classList.remove("notActive")
      card.firstChild.classList.add("active")     
    }
  }

  // Renders the JSX of the cards
  return (        
    <div className="cardBox col s12 m4" id={props.storyID}>
      {props.active ? (
        <span className="active"></span>
      ) : (
        <span className="notActive"></span>
      )}
      <div className="card">
         <div className="card-content white-text">
          <div className="row adminPanel">
            <div className="col s8">
              <div className="switch" onChange={toggleSwitch}>
                <label>
                  Inactief
                  {props.active ? (
                    <input type="checkbox" defaultChecked></input> 
                  ) : (
                    <input type="checkbox"></input> 
                  )}
                  <span className="lever"></span>
                  Actief
                </label>
              </div>
            </div>
            <div className="col s4">
              <div className="right-buttons">
                  <div className="showItem" onClick={showItem}>
                      <FontAwesomeIcon icon={faEye} />
                  </div>
                <div className="deleteItem" onClick={deleteItem}>
                  <FontAwesomeIcon icon={faTrashAlt} />
                </div>
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
          {/* Loops through the feedback and makes a bar for each feedbackType */}
          {Object.entries(createBarData(props)).map((element, index) =>
              <span key={element[0]}>
                <div key={element[0]}>
                  <div className="col s3">
                    <p>{element[0]}</p>
                  </div>
                  <div className="col s9">
                    <div className="bar">
                        <HSBar
                          data={element[1]}
                        />
                    </div>
                  </div>
                </div>
              </span>         
            )}
          </div>
         </div>
      </div>
    </div>
  )


};
// Exports data to use
export default card;