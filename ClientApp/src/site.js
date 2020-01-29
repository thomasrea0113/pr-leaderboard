// Elements needed to run the site, including frontend dependencies like bootstrap that the .NET core views rely on

require('expose-loader?React!react'); // eslint-disable-line import/no-webpack-loader-syntax
require('expose-loader?ReactDOM!react-dom'); // eslint-disable-line import/no-webpack-loader-syntax

import 'bootstrap/dist/css/bootstrap.css';
import './css/custom.css'
