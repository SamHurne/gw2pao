"use strict";

var gulp = require('gulp');
var concat = require('gulp-concat');
var uglify = require('gulp-uglify');
var cssnano = require('gulp-cssnano');
var htmlmin = require('gulp-htmlmin');

// Gulp tasks =================================================================
gulp.task('default', ['all']);
gulp.task('all', ['js', 'css']);

gulp.task('js', function() {
   return gulp.src('js/*.js')
      .pipe(concat("site.min.js"))
      .pipe(uglify())
      .pipe(gulp.dest('./'));
});
gulp.task('css', function() {
   return gulp.src('css/*.css')
      .pipe(concat("site.min.css"))
      .pipe(cssnano())
      .pipe(gulp.dest('./'));
});

// Watch tasks =================================================================
gulp.task('watch', ['watch-js', 'watch-css']);
gulp.task('watch-js', function() {
   gulp.watch('js/**/*.js', ['js']);
});
gulp.task('watch-css', function() {
   gulp.watch('css/**/*.css', ['css']);
});