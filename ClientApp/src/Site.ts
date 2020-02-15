/* eslint-disable import/no-unresolved */
/* eslint-disable import/no-webpack-loader-syntax */

import './jquery-unobtrusive-ajax-extensions';

import 'expose-loader?React!react';
import 'expose-loader?ReactDOM!react-dom';

import 'expose-loader?jQuery!jquery';
import 'jquery-validation';
import 'jquery-validation-unobtrusive';
import 'jquery-ajax-unobtrusive';

import 'bootstrap/dist/css/bootstrap.css';
import './scss/site.scss';
import '@fortawesome/fontawesome-free/js/all';
