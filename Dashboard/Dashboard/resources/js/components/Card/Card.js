import React, { useEffect, useState } from 'react';
import HSBar from 'react-horizontal-stacked-bar-chart';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faTrashAlt, faEye } from '@fortawesome/free-solid-svg-icons';

const card = (props) => {
  const deleteItem = () => props.onDelete(props.storyID);
  const showItem = () => props.onShow(props.storyID);

  function createBarData(toDataArray){
    var data = [];
    var colors = ["#009688","#84c7c1","#ff0043","#e7edf2"];

    var toloop = toDataArray.cardInfo[0];
    toloop.forEach(function(element) {
      if(toDataArray.storyID == element.storyId){
        var i = 0;
        element.feedback.forEach(function(feedbackItem) {
          if (typeof data[feedbackItem.oneWord] === 'undefined') {
            data[feedbackItem.oneWord] = [];
          }

          Object.entries(feedbackItem.feedback).map((element, index) =>
              data[feedbackItem.oneWord][index] ={value: element[1].count, description: element[1].count, color: colors[index]}
          );
        });
      }
    });
    
    return data;
  }

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

export default card;