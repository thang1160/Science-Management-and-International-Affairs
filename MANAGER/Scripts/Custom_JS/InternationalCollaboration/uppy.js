﻿//var KTUppy = function () {
const Tus = Uppy.Tus;
//const ProgressBar = Uppy.ProgressBar;
const StatusBar = Uppy.StatusBar;
const FileInput = Uppy.FileInput;
const Informer = Uppy.Informer;

// to get uppy companions working, please refer to the official documentation here: https://uppy.io/docs/companion/
//const Dashboard = Uppy.Dashboard;
//const Dropbox = Uppy.Dropbox;
//const GoogleDrive = Uppy.GoogleDrive;
//const Instagram = Uppy.Instagram;
//const Webcam = Uppy.Webcam;

// Private functions

function initUppy5(data) {
    // Uppy variables
    // For more info refer: https://uppy.io/
    var elemId = data;
    var id = '#' + elemId;
    var $statusBar = $(id + ' .uppy-status');
    var $uploadedList = $(id + ' .uppy-list');
    var timeout;

    var uppyMin = Uppy.Core({
        debug: true,
        autoProceed: true,
        showProgressDetails: true,
        restrictions: {
            maxFileSize: 1000000, // 1mb
            maxNumberOfFiles: 5,
            minNumberOfFiles: 1
        }
    });

    uppyMin.use(FileInput, { target: id + ' .uppy-wrapper', pretty: false });
    uppyMin.use(Informer, { target: id + ' .uppy-informer' });

    // demo file upload server
    uppyMin.use(Tus, { endpoint: 'https://master.tus.io/files/' });
    uppyMin.use(StatusBar, {
        target: id + ' .uppy-status',
        hideUploadButton: true,
        hideAfterFinish: false
    });

    $(id + ' .uppy-FileInput-input').addClass('uppy-input-control').attr('id', elemId + '_input_control');
    $(id + ' .uppy-FileInput-container').append('<label class="uppy-input-label btn btn-light-primary btn-sm btn-bold" for="' + (elemId + '_input_control') + '">Attach files</label>');

    var $fileLabel = $(id + ' .uppy-input-label');

    uppyMin.on('upload', function () {
        $fileLabel.text("Uploading...");
        $statusBar.addClass('uppy-status-ongoing');
        $statusBar.removeClass('uppy-status-hidden');
        clearTimeout(timeout);
    });

    uppyMin.on('complete', function (file) {
        $.each(file.successful, function (index, value) {
            var sizeLabel = "bytes";
            var filesize = value.size;
            if (filesize > 1024) {
                filesize = filesize / 1024;
                sizeLabel = "kb";

                if (filesize > 1024) {
                    filesize = filesize / 1024;
                    sizeLabel = "MB";
                }
            }
            var uploadListHtml = '<div class="uppy-list-item" data-id="' + value.id + '"><div class="uppy-list-label">' + value.name + ' (' + Math.round(filesize, 2) + ' ' + sizeLabel + ')</div><span class="uppy-list-remove" data-id="' + value.id + '"><i class="flaticon2-cancel-music"></i></span></div>';
            $uploadedList.append(uploadListHtml);
        });

        $fileLabel.text("Add more files");

        $statusBar.addClass('uppy-status-hidden');
        $statusBar.removeClass('uppy-status-ongoing');
    });

    $(document).on('click', id + ' .uppy-list .uppy-list-remove', function () {
        var itemId = $(this).attr('data-id');
        uppyMin.removeFile(itemId);
        $(id + ' .uppy-list-item[data-id="' + itemId + '"').remove();
    });
}


$(document).ready(function () {
    $('.uppy-input-label').removeClass('btn-sm');
    initUppy5('add_officer_upload');
    initUppy5('edit_officer_upload');
    initUppy5('change_status_upload');
})

