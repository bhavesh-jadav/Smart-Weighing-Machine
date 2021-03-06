﻿/// <binding AfterBuild='minify_js, minify_mail_html' />
/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require('gulp');
var uglify = require('gulp-uglify');
var htmlmin = require('gulp-htmlmin');
var ngAnnotate = require('gulp-ng-annotate');
var minifycss = require('gulp-clean-css');

gulp.task('minify_js', function () {
    // place code for your default task here

    return gulp.src("wwwroot/js/*.js")
               .pipe(ngAnnotate())
               .pipe(uglify())
               .pipe(gulp.dest("wwwroot/distribution/js"));
});
gulp.task('minify_mail_html', function () {
    return gulp.src('MailBodies/*.html')
      .pipe(htmlmin({ collapseWhitespace: true }))
      .pipe(gulp.dest('MailBodies/_mailBodies'));
});
gulp.task('minify_css', function () {
    return gulp.src('wwwroot/css/*.css')
    .pipe(minifycss({ compatibility: 'ie8' }))
    .pipe(gulp.dest('wwwroot/distribution/css'));
});