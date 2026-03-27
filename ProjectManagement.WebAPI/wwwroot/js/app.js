var API_BASE = window.location.origin + '/api';

new Vue({
    el: '#app',
    data: {
        currentStep: 0,
        steps: [
            { title: 'Основная информация', completed: false },
            { title: 'Компании', completed: false },
            { title: 'Руководитель', completed: false },
            { title: 'Исполнители', completed: false },
            { title: 'Документы', completed: false }
        ],
        project: {
            name: '',
            startDate: '',
            endDate: '',
            priority: 5,
            customerCompany: '',
            executorCompany: '',
            projectManagerId: null,
            executorIds: [],
            executors: []
        },
        managerSearch: '',
        executorSearch: '',
        managerResults: [],
        executorResults: [],
        selectedManager: null,
        uploadedFiles: [],
        allEmployees: [],
        showManagerDropdown: false,
        showExecutorDropdown: false,
        isLoading: false
    },
    computed: {
        priorityClass: function() {
            var p = this.project.priority;
            if (p <= 3) return 'priority-low';
            if (p <= 7) return 'priority-medium';
            return 'priority-high';
        }
    },
    mounted: function() {
        this.loadEmployees();
    },
    methods: {
        loadEmployees: function() {
            var self = this;
            axios.get(API_BASE + '/Employees')
                .then(function(response) {
                    self.allEmployees = response.data;
                    console.log('Загружено сотрудников:', self.allEmployees.length);
                })
                .catch(function(error) {
                    console.error('Ошибка загрузки сотрудников:', error);
                });
        },
        searchManagers: function() {
            var self = this;
            if (this.managerSearch.length < 2) {
                this.managerResults = this.allEmployees;
                return;
            }
            axios.get(API_BASE + '/Employees/search?term=' + encodeURIComponent(this.managerSearch))
                .then(function(response) {
                    self.managerResults = response.data;
                })
                .catch(function(error) {
                    console.error('Ошибка поиска:', error);
                });
        },
        searchExecutors: function() {
            var self = this;
            if (this.executorSearch.length < 2) {
                var filtered = [];
                for (var i = 0; i < this.allEmployees.length; i++) {
                    var emp = this.allEmployees[i];
                    if (this.project.executorIds.indexOf(emp.id) === -1) {
                        filtered.push(emp);
                    }
                }
                this.executorResults = filtered;
                return;
            }
            axios.get(API_BASE + '/Employees/search?term=' + encodeURIComponent(this.executorSearch))
                .then(function(response) {
                    var results = response.data;
                    var filtered = [];
                    for (var i = 0; i < results.length; i++) {
                        var emp = results[i];
                        if (self.project.executorIds.indexOf(emp.id) === -1) {
                            filtered.push(emp);
                        }
                    }
                    self.executorResults = filtered;
                })
                .catch(function(error) {
                    console.error('Ошибка поиска:', error);
                });
        },
        openManagerDropdown: function() {
            this.showManagerDropdown = true;
            this.managerResults = this.allEmployees;
        },
        openExecutorDropdown: function() {
            this.showExecutorDropdown = true;
            var filtered = [];
            for (var i = 0; i < this.allEmployees.length; i++) {
                var emp = this.allEmployees[i];
                if (this.project.executorIds.indexOf(emp.id) === -1) {
                    filtered.push(emp);
                }
            }
            this.executorResults = filtered;
        },
        closeManagerDropdown: function() {
            var self = this;
            setTimeout(function() {
                self.showManagerDropdown = false;
            }, 200);
        },
        closeExecutorDropdown: function() {
            var self = this;
            setTimeout(function() {
                self.showExecutorDropdown = false;
            }, 200);
        },
        selectManager: function(employee) {
            this.selectedManager = employee;
            this.project.projectManagerId = employee.id;
            this.managerSearch = employee.fullName;
            this.managerResults = [];
            this.showManagerDropdown = false;
        },
        addExecutor: function(employee) {
            if (this.project.executorIds.indexOf(employee.id) === -1) {
                this.project.executorIds.push(employee.id);
                this.project.executors.push(employee);
            }
            this.executorSearch = '';
            this.executorResults = [];
            this.showExecutorDropdown = false;
        },
        removeExecutor: function(employeeId) {
            var index = this.project.executorIds.indexOf(employeeId);
            if (index !== -1) {
                this.project.executorIds.splice(index, 1);
                for (var i = 0; i < this.project.executors.length; i++) {
                    if (this.project.executors[i].id === employeeId) {
                        this.project.executors.splice(i, 1);
                        break;
                    }
                }
            }
        },
        goToStep: function(step) {
            if (step <= this.currentStep) {
                this.currentStep = step;
                return;
            }
            if (this.validateStep(this.currentStep)) {
                this.steps[this.currentStep].completed = true;
                this.currentStep = step;
            }
        },
        nextStep: function() {
            if (this.validateStep(this.currentStep)) {
                this.steps[this.currentStep].completed = true;
                if (this.currentStep < 4) {
                    this.currentStep++;
                }
            }
        },
        prevStep: function() {
            if (this.currentStep > 0) {
                this.currentStep--;
            }
        },
        validateStep: function(step) {
            if (step === 0) {
                if (!this.project.name) {
                    alert('Введите название проекта');
                    return false;
                }
                if (!this.project.startDate) {
                    alert('Выберите дату начала проекта');
                    return false;
                }
                if (!this.project.endDate) {
                    alert('Выберите дату окончания проекта');
                    return false;
                }
                if (this.project.startDate > this.project.endDate) {
                    alert('Дата окончания должна быть позже даты начала');
                    return false;
                }
                return true;
            }
            if (step === 1) {
                if (!this.project.customerCompany) {
                    alert('Введите компанию-заказчика');
                    return false;
                }
                if (!this.project.executorCompany) {
                    alert('Введите компанию-исполнителя');
                    return false;
                }
                return true;
            }
            if (step === 2) {
                if (!this.project.projectManagerId) {
                    alert('Выберите руководителя проекта');
                    return false;
                }
                return true;
            }
            return true;
        },
        triggerFileInput: function() {
            var fileInput = this.$refs.fileInput;
            if (fileInput) {
                fileInput.click();
            }
        },
        onDragOver: function(event) {
            var zone = event.target.closest('.dropzone');
            if (zone) zone.classList.add('drag');
        },
        onDragLeave: function(event) {
            var zone = event.target.closest('.dropzone');
            if (zone) zone.classList.remove('drag');
        },
        onDrop: function(event) {
            var zone = event.target.closest('.dropzone');
            if (zone) zone.classList.remove('drag');
            var files = Array.from(event.dataTransfer.files);
            this.addFiles(files);
        },
        onFileSelect: function(event) {
            var files = Array.from(event.target.files);
            this.addFiles(files);
        },
        addFiles: function(files) {
            for (var i = 0; i < files.length; i++) {
                this.uploadedFiles.push(files[i]);
            }
        },
        formatFileSize: function(bytes) {
            if (bytes === 0) return '0 Байт';
            var k = 1024;
            var sizes = ['Байт', 'КБ', 'МБ', 'ГБ'];
            var i = Math.floor(Math.log(bytes) / Math.log(k));
            return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
        },
        submitProject: function() {
            var self = this;
            if (!this.validateStep(0)) return;
            if (!this.validateStep(1)) return;
            if (!this.validateStep(2)) return;
            
            var startDate = this.project.startDate;
            var endDate = this.project.endDate;
            
            if (startDate && startDate.length === 10) {
                startDate = startDate + 'T00:00:00';
            }
            if (endDate && endDate.length === 10) {
                endDate = endDate + 'T00:00:00';
            }
            
            var projectData = {
                name: this.project.name,
                customerCompany: this.project.customerCompany,
                executorCompany: this.project.executorCompany,
                startDate: startDate,
                endDate: endDate,
                priority: this.project.priority,
                projectManagerId: this.project.projectManagerId,
                executorIds: this.project.executorIds
            };
            
            this.isLoading = true;
            
            axios.post(API_BASE + '/Projects', projectData)
                .then(function(response) {
                    alert('Проект "' + self.project.name + '" успешно создан! ID: ' + response.data.id);
                    self.resetForm();
                })
                .catch(function(error) {
                    var errorMsg = error.message;
                    if (error.response && error.response.data) {
                        if (error.response.data.message) {
                            errorMsg = error.response.data.message;
                        }
                    }
                    alert('Ошибка: ' + errorMsg);
                })
                .finally(function() {
                    self.isLoading = false;
                });
        },
        resetForm: function() {
            this.currentStep = 0;
            this.project = {
                name: '',
                startDate: '',
                endDate: '',
                priority: 5,
                customerCompany: '',
                executorCompany: '',
                projectManagerId: null,
                executorIds: [],
                executors: []
            };
            this.managerSearch = '';
            this.executorSearch = '';
            this.managerResults = [];
            this.executorResults = [];
            this.selectedManager = null;
            this.uploadedFiles = [];
            for (var i = 0; i < this.steps.length; i++) {
                this.steps[i].completed = false;
            }
        }
    }
});