/* eslint-disable import/no-unresolved */
/* eslint-disable import/no-webpack-loader-syntax */

// react
import 'expose-loader?React!react';
import 'expose-loader?ReactDOM!react-dom';

// jQuery
import 'expose-loader?jQuery!jquery';
import 'jquery-validation';
import 'jquery-validation-unobtrusive';
import 'jquery-ajax-unobtrusive';
import './jquery.unobtrusive-ajax.extended';

// Styles
import './scss/site.scss';
import '@fortawesome/fontawesome-free/js/all';
